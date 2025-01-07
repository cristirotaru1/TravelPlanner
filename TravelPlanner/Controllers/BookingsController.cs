using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TravelPlanner.Models;

namespace TravelPlanner.Controllers
{
    public class BookingsController : Controller
    {
        string connectionString = "Server=localhost;Database=TravelDB;Trusted_Connection=True;";
        UserController userController = new UserController();

        // GET: Bookings
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StoreAccommodationIdAndRedirect(int accommodationId, string actionName, string controllerName)
        {
            Session["AccommodationId"] = accommodationId;
            return RedirectToAction(actionName, controllerName);
        }


        [HttpPost]
        public ActionResult BookAccommodation(BookAccommodationViewModel model, int accommodationId)
        {
            int bookingId = 0;
            if (ModelState.IsValid)
            {
                int userId = 0;

                //User
                var authCookieUser = Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authCookieUser != null)
                {
                    try
                    {
                        var ticket = FormsAuthentication.Decrypt(authCookieUser.Value);
                        int.TryParse(ticket.Name, out userId);
                    }
                    catch (CryptographicException ex)
                    {
                        return Content("You need to be authenticated in order to book an accommodation.");
                    }
                }

                //Accommodation
                if (Session["AccommodationId"] != null)
                {
                    accommodationId = (int)Session["AccommodationId"];
                }

                // Validate dates
                DateTime currentDate = DateTime.Now.Date;
                if (model.CheckInDate < currentDate)
                {
                    return Content("Please select Check In Date to be in the future");
                }

                if (model.CheckOutDate <= model.CheckInDate)
                {
                    return Content("Please select Check-Out Date to be after the Check-In Date");
                }

                if (userId != 0 && accommodationId != 0)
                {
                    Bookings bookings = new Bookings
                    {
                        UserId = userId,
                        AccommodationId = accommodationId,
                        CheckInDate = model.CheckInDate,
                        CheckOutDate = model.CheckOutDate,
                    };

                    TimeSpan stayDuration = bookings.CheckOutDate.Date.Subtract(bookings.CheckInDate.Date);
                    int numberOfNights = stayDuration.Days;
                    decimal totalPrice = model.Price * numberOfNights;

                    bookings.TotalPrice = totalPrice;
                    bookings.CreatedAt = DateTime.Now;

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        string queryInsertBooking = "INSERT INTO BookingsAccommodations (UserId, AccommodationId, CheckInDate, CheckOutDate, TotalPrice, CreatedAt) " +
                                                    "VALUES (@UserId, @AccommodationId, @CheckInDate, @CheckOutDate, @TotalPrice, @CreatedAt)";

                        using (SqlCommand insertCommand = new SqlCommand(queryInsertBooking, conn))
                        {
                            insertCommand.Parameters.AddWithValue("@UserId", bookings.UserId);
                            insertCommand.Parameters.AddWithValue("@AccommodationId", bookings.AccommodationId);
                            insertCommand.Parameters.AddWithValue("@CheckInDate", bookings.CheckInDate);
                            insertCommand.Parameters.AddWithValue("@CheckOutDate", bookings.CheckOutDate);
                            insertCommand.Parameters.AddWithValue("@TotalPrice", bookings.TotalPrice);
                            insertCommand.Parameters.AddWithValue("@CreatedAt", bookings.CreatedAt);

                            insertCommand.ExecuteScalar();
                        }
                    }
                    
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string queryGetBookingId = "SELECT TOP 1 BookingId FROM BookingsAccommodations ORDER BY BookingId DESC";
                        using (SqlCommand command = new SqlCommand(queryGetBookingId, connection))
                        {
                            object result = command.ExecuteScalar();
                            if (result != null)
                            {
                                bookingId = (int)result;
                            }
                        }

                        string queryGetBooking = "SELECT BookingId, UserId, AccommodationId, CheckInDate, CheckOutDate, TotalPrice, CreatedAt " +
                                                 "FROM BookingsAccommodations " +
                                                 "WHERE BookingId = @BookingId";

                        using (SqlCommand getBookingCommand = new SqlCommand(queryGetBooking, connection))
                        {
                            getBookingCommand.Parameters.AddWithValue("@BookingId", bookingId);

                            using (SqlDataReader reader = getBookingCommand.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    bookings = new Bookings
                                    {
                                        BookingId = (int)reader["BookingId"],
                                        UserId = (int)reader["UserId"],
                                        AccommodationId = (int)reader["AccommodationId"],
                                        CheckInDate = (DateTime)reader["CheckInDate"],
                                        CheckOutDate = (DateTime)reader["CheckOutDate"],
                                        TotalPrice = (decimal)reader["TotalPrice"],
                                        CreatedAt = (DateTime)reader["CreatedAt"]
                                    };
                                    return RedirectToAction("Confirmation", new { bookingId = bookings.BookingId });
                                }
                            }
                        }
                    }
                }
            }
            return RedirectToAction("Confirmation", new { id = model.AccommodationId });
        }


        public ActionResult BookAccommodationView(int id)
        {
            BookAccommodationViewModel model = new BookAccommodationViewModel();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT AccommodationName, AccommodationDescription, AccommodationType, AccommodationLocation, UserId, Price FROM Accommodations WHERE AccommodationId = @Id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            model.AccommodationName = (string)reader["AccommodationName"];
                            model.AccommodationDescription = (string)reader["AccommodationDescription"];
                            model.AccommodationType = (string)reader["AccommodationType"];
                            model.AccommodationLocation = (string)reader["AccommodationLocation"];
                            model.Price = (decimal)reader["Price"];
                            model.OwnerName = userController.GetUserNameById((int)reader["UserId"]);
                        }
                    }
                }
            }

            return View(model);
        }

        public ActionResult Confirmation(int bookingId)
        {
            Bookings booking;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT BookingId, UserId, AccommodationId, CheckInDate, CheckOutDate, TotalPrice, CreatedAt FROM BookingsAccommodations WHERE BookingId = @BookingId";

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@BookingId", bookingId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            booking = new Bookings
                            {
                                BookingId = (int)reader["BookingId"],
                                UserId = (int)reader["UserId"],
                                AccommodationId = (int)reader["AccommodationId"],
                                CheckInDate = (DateTime)reader["CheckInDate"],
                                CheckOutDate = (DateTime)reader["CheckOutDate"],
                                TotalPrice = (decimal)reader["TotalPrice"],
                                CreatedAt = (DateTime)reader["CreatedAt"]
                            };
                        }
                        else
                        {
                            return RedirectToAction("BookingNotFound");
                        }
                    }
                }
            }

            return View(booking);
        }   
    }
}