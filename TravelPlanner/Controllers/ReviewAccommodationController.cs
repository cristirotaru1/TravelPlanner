using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.EnterpriseServices;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TravelPlanner.Models;

namespace TravelPlanner.Controllers
{
    public class ReviewAccommodationController : Controller
    {
        string connectionString = "Server=localhost;Database=TravelDB;Trusted_Connection=True;";

        UserController userController = new UserController();

        // GET: ReviewAccommodation
        public ActionResult Index()
        {
            return View();
        }

        // add review

        public ActionResult StoreAccommodationIdAndRedirectReview(int accommodationId, string actionName, string controllerName)
        {
            Session["AccommodationId"] = accommodationId;
            return RedirectToAction(actionName, controllerName);
        }


        [HttpPost]
        public ActionResult LeaveReview(AccommodationReviewViewModel model, int accommodationId)
        {
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
                        return Content("You need to be authenticated in order to write a review.");
                    }
                }

                //Accommodation
                if (Session["AccommodationId"] != null)
                {
                    accommodationId = (int)Session["AccommodationId"];
                }

                if (userId != 0 && accommodationId != 0)
                {
                    ReviewAccommodation review = new ReviewAccommodation
                    {
                        UserId = userId,
                        AccommodationId = accommodationId,
                        Rating = model.Rating,
                        Comment = model.Comment,
                        FullName = model.FullName
                    };
                    review.CreatedAt = DateTime.Now;

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string queryInsert = "INSERT INTO ReviewsAccommodations(UserId, AccommodationId, Rating, Comment, CreatedAt) VALUES (@UserId, @AccommodationId, @Rating, @Comment, @CreatedAt)";

                        using (SqlCommand command = new SqlCommand(queryInsert, connection))
                        {
                            command.Parameters.AddWithValue("@UserId", review.UserId);
                            command.Parameters.AddWithValue("@AccommodationId", review.AccommodationId);
                            command.Parameters.AddWithValue("@Rating", review.Rating);
                            command.Parameters.AddWithValue("@Comment", review.Comment);
                            command.Parameters.AddWithValue("@CreatedAt", review.CreatedAt);

                            command.ExecuteScalar();
                        }
                    }
                }
            }
            else
            {
                return Content("Failed to submit review");
            }
            return Content("Review submitted");
        }

        public ActionResult ReviewAccommodationView(int id)
        {
            AccommodationReviewViewModel review = new AccommodationReviewViewModel();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT AccommodationName FROM Accommodations WHERE AccommodationId = @Id";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            review.AccommodationId = id;
                            review.AccommodationName = (string)reader["AccommodationName"];
                        }
                    }
                }
            }
            return View(review);
        }

        [HttpGet]
        public ActionResult DisplayReview(int accommodationId)
        {
            List<ReviewAccommodation> reviewAccommodations = new List<ReviewAccommodation>();
            if (Session["AccommodationId"] != null)
            {
                accommodationId = (int)Session["AccommodationId"];
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string queryToDisplayReview = "SELECT * FROM ReviewsAccommodations WHERE AccommodationId = @AccommodationId";
                    using (SqlCommand command = new SqlCommand(queryToDisplayReview, conn))
                    {
                        command.Parameters.AddWithValue("@AccommodationId", accommodationId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ReviewAccommodation review = new ReviewAccommodation
                                {
                                    ReviewId = (int)reader["ReviewId"],
                                    Rating = (int)reader["Rating"],
                                    UserId = (int)reader["UserId"],
                                    Comment = reader["Comment"].ToString(),
                                    CreatedAt = (DateTime)reader["CreatedAt"],
                                    FullName = userController.GetUserNameById((int)reader["UserId"])
                                };
                                reviewAccommodations.Add(review);
                            }
                        }
                    }
                }
                return View("DisplayReviewAccommodation", reviewAccommodations);
            }
            catch (Exception e)
            {
                return View("Error" + e);
            }
        }

        [HttpPost]
        public ActionResult DeleteReview(int reviewId, int accommodationId)
        {
            if (Session["AccommodationId"] == null)
            {
                Session["AccommodationId"] = accommodationId;
            }

            System.Diagnostics.Debug.WriteLine(reviewId);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string deleteQuery = "DELETE FROM ReviewsAccommodations WHERE ReviewId = @ReviewId";
                using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@ReviewId", reviewId);
                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("DisplayReview", new { accommodationId });
        }
    }
}