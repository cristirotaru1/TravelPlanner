using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TravelPlanner.Models;

namespace TravelPlanner.Controllers
{
    public class AccommodationController : Controller
    {
        string connectionString = "Server=localhost;Database=TravelDB;Trusted_Connection=True;";
        UserController userController = new UserController();
        // GET: Accomodations
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult NewAccomodation()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateAccomodation(Accommodation accommodations)
        {
            CreateNewAccommodation(accommodations);

            return Content("Added accomodation"); ;
        }

        public void CreateNewAccommodation(Accommodation accommodations)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string insertQuery = "INSERT INTO Accommodations (AccommodationName, AccommodationType, AccommodationLocation, AccommodationDescription, UserId, Price, CreatedAt) " +
                                     "VALUES (@AccommodationName, @AccommodationType, @AccommodationLocation, @AccommodationDescription, @UserId, @Price, @CreatedAt)";

                using (SqlCommand command = new SqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@AccommodationName", accommodations.AccomodationName);
                    command.Parameters.AddWithValue("@AccommodationType", accommodations.AccommodationType);
                    command.Parameters.AddWithValue("@AccommodationLocation", accommodations.AccommodationLocation);
                    command.Parameters.AddWithValue("@AccommodationDescription", accommodations.AccomodationDescription);
                    command.Parameters.AddWithValue("@UserId", accommodations.UserId);
                    command.Parameters.AddWithValue("@Price", accommodations.Price);
                    command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                    command.ExecuteNonQuery();
                }
            }
        }


        // GET: Accommodation
        public ActionResult SearchAccommodation(string searchAccommodation)
        {
            List<Accommodation> accommodations = new List<Accommodation>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string queryToSearch = "SELECT * FROM Accommodations WHERE AccommodationLocation LIKE @searchAccommodation";
                using (SqlCommand command = new SqlCommand(queryToSearch, conn))
                {
                    command.Parameters.AddWithValue("@searchAccommodation", "%" + searchAccommodation + "%");
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Accommodation accommodation = new Accommodation
                        {
                            AccommodationId = (int)reader["AccommodationId"],
                            AccomodationName = reader["AccommodationName"].ToString(),
                            AccommodationType = reader["AccommodationType"].ToString(),
                            AccommodationLocation = reader["AccommodationLocation"].ToString(),
                            UserId = (int)reader["UserId"],
                            OwnerName = userController.GetUserNameById((int)reader["UserId"]),
                            Price = (decimal)reader["Price"],
                            AccomodationDescription = reader["AccommodationDescription"].ToString(),
                            CreatedAt = (DateTime)reader["CreatedAt"],
                        };
                        accommodations.Add(accommodation);
                        // TODO: Create field UserId for removing the accomodations and showing the Owner Name
                    }
                }
            }
            return View("AccommodationView", accommodations);
        }


        // View all accommodations

        [HttpGet]
        public ActionResult AccommodationView()
        {
            List<Accommodation> accommodations = GetAllAccommodations();
            return View(accommodations);
        }


        public List<Accommodation> GetAllAccommodations()
        {
            List<Accommodation> accommodations = new List<Accommodation>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string selectQuery = "SELECT * FROM Accommodations";

                using (SqlCommand command = new SqlCommand(selectQuery, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Accommodation accommodation = new Accommodation();

                        accommodation.AccommodationId = (int)reader["AccommodationId"];
                        accommodation.AccomodationName = (string)reader["AccommodationName"];
                        accommodation.AccommodationType = (string)reader["AccommodationType"];
                        accommodation.AccommodationLocation = (string)reader["AccommodationLocation"];
                        accommodation.AccomodationDescription = (string)reader["AccommodationDescription"];
                        accommodation.UserId = (int)reader["UserId"];
                        accommodation.OwnerName = userController.GetUserNameById((int)reader["UserId"]);
                        accommodation.Price = (decimal)reader["Price"];
                        accommodation.CreatedAt = (DateTime)reader["CreatedAt"];

                        accommodations.Add(accommodation);
                    }
                }
            }

            return accommodations;
        }

        // DELETE ACCOMMODATION
        [HttpPost]
        public ActionResult DeleteAccommodation(int accommodationId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string deleteBookingsQuery = "DELETE FROM BookingsAccommodations WHERE AccommodationId = @AccommodationId";
                using (SqlCommand deleteBookingsCommand = new SqlCommand(deleteBookingsQuery, connection))
                {
                    deleteBookingsCommand.Parameters.AddWithValue("@AccommodationId", accommodationId);
                    deleteBookingsCommand.ExecuteNonQuery();
                }

                string deleteReviewsQuery = "DELETE FROM ReviewsAccommodations WHERE AccommodationId = @AccommodationId";
                using (SqlCommand deleteReviewsCommand = new SqlCommand(deleteReviewsQuery, connection))
                {
                    deleteReviewsCommand.Parameters.AddWithValue("@AccommodationId", accommodationId);
                    deleteReviewsCommand.ExecuteNonQuery();
                }

                string deleteAccommodationQuery = "DELETE FROM Accommodations WHERE AccommodationId = @AccommodationId";
                using (SqlCommand deleteAccommodationCommand = new SqlCommand(deleteAccommodationQuery, connection))
                {
                    deleteAccommodationCommand.Parameters.AddWithValue("@AccommodationId", accommodationId);
                    deleteAccommodationCommand.ExecuteNonQuery();
                }
            }

            return RedirectToAction("AccommodationView");
        }
    }
}