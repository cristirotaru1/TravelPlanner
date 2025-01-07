using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.EnterpriseServices;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TravelPlanner.Models;

namespace TravelPlanner.Controllers
{
    public class ReviewActivityController : Controller
    {
        string connectionString = "Server=localhost;Database=TravelDB;Trusted_Connection=True;";
        UserController userController = new UserController();

        // GET: ReviewActivity
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StoreAccommodationIdAndRedirectReviewActivity(int activityId, string actionName, string controllerName)
        {
            Session["ActivityId"] = activityId;
            return RedirectToAction(actionName, controllerName);
        }

        [HttpPost]
        public ActionResult LeaveReview(ActivityReviewViewModel model, int activityId)
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

                //Activity
                if (Session["ActivityId"] != null)
                {
                    activityId = (int)Session["ActivityId"];
                }

                if (userId != 0 && activityId != 0)
                {
                    ReviewActivity review = new ReviewActivity
                    {
                        UserId = userId,
                        ActivityId = activityId,
                        Rating = model.Rating,
                        Comment = model.Comment,
                        FullName = model.FullName
                    };
                    review.CreatedAt = DateTime.Now;

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string queryInsert = "INSERT INTO ReviewsActivities(UserId, ActivityId, Rating, Comment, CreatedAt) VALUES (@UserId, @ActivityId, @Rating, @Comment, @CreatedAt)";

                        using (SqlCommand command = new SqlCommand(queryInsert, connection))
                        {
                            command.Parameters.AddWithValue("@UserId", review.UserId);
                            command.Parameters.AddWithValue("@ActivityId", review.ActivityId);
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


        public ActionResult ReviewActivityView(int id)
        {
            ActivityReviewViewModel review = new ActivityReviewViewModel();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT ActivityName FROM Activities WHERE ActivityId = @Id";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            review.ActivityId = id;
                            review.ActivityName = (string)reader["ActivityName"];
                        }
                    }
                }
            }
            return View(review);
        }


        [HttpGet]
        public ActionResult DisplayReview(int activityId)
        {
            List<ReviewActivity> reviewActivities = new List<ReviewActivity>();
            if (Session["ActivityId"] != null)
            {
                activityId = (int)Session["ActivityId"];
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string queryToDisplayReview = "SELECT * FROM ReviewsActivities WHERE ActivityId = @ActivityId";
                    using (SqlCommand command = new SqlCommand(queryToDisplayReview, conn))
                    {
                        command.Parameters.AddWithValue("@ActivityId", activityId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ReviewActivity review = new ReviewActivity
                                {
                                    ReviewId = (int)reader["ReviewId"],
                                    UserId = (int)reader["UserId"],
                                    ActivityId = (int)reader["ActivityId"],
                                    Rating = (int)reader["Rating"],
                                    Comment = reader["Comment"].ToString(),
                                    CreatedAt = (DateTime)reader["CreatedAt"],
                                    FullName = userController.GetUserNameById((int)reader["UserId"])
                                };
                                reviewActivities.Add(review);
                            }
                        }
                    }
                }
                return View("DisplayReviewActivity", reviewActivities);
            }
            catch (Exception e)
            {
                return View("Error" + e);
            }
        }

        [HttpPost]
        public ActionResult DeleteReview(int reviewId, int activityId)
        {
            if (Session["ActivityId"] == null)
            {
                Session["ActivityId"] = activityId;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string deleteQuery = "DELETE FROM ReviewsActivities WHERE ReviewId = @ReviewId";
                using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@ReviewId", reviewId);
                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("DisplayReview", new { activityId });
        }
    }
}