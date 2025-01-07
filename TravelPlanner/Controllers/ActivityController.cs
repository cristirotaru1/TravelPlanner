using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TravelPlanner.Models;

namespace TravelPlanner.Controllers
{
    public class ActivityController : Controller
    {
        string connectionString = "Server=localhost;Database=TravelDB;Trusted_Connection=True;";

        // GET: Activity
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult NewActivity()
        {
            return View();
        }

        //ADD ACTIVITY
        [HttpPost]
        public ActionResult CreateActivity(Models.Activity activity)
        {
            CreateNewActivity(activity);
            return Content("Activity added"); ;
        }

        public void CreateNewActivity(Models.Activity activity)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string insertQuery = "INSERT INTO Activities (ActivityName, ActivityType, ActivityDescription, Price, CreatedAt)  " +
                                     "VALUES (@ActivityName, @ActivityType, @ActivityDescription, @Price, @CreatedAt)";

                using (SqlCommand command = new SqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@ActivityName", activity.ActivityName);
                    command.Parameters.AddWithValue("@ActivityType", activity.ActivityType);
                    command.Parameters.AddWithValue("@ActivityDescription", activity.ActivityDescription);
                    command.Parameters.AddWithValue("@Price", activity.Price);
                    command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                    command.ExecuteNonQuery();
                }
            }
        }

        // SEARCH
        [HttpGet]
        public ActionResult SearchActivity(string searchActivity)
        {
            
            List<Activity> activities = new List<Activity>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string queryToSearch = "SELECT * FROM Activities WHERE ActivityName LIKE @searchActivity";
                using (SqlCommand command = new SqlCommand(queryToSearch, conn))
                {
                    command.Parameters.AddWithValue("@searchActivity", "%" + searchActivity + "%");
                    conn.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Activity activity = new Activity
                        {
                            ActivityId = (int)reader["ActivityId"],
                            ActivityName = reader["ActivityName"].ToString(),
                            ActivityType = reader["ActivityType"].ToString(),
                            ActivityDescription = reader["ActivityDescription"].ToString(),
                            Price = (decimal)reader["Price"],
                            CreatedAt = (DateTime)reader["CreatedAt"],
                        };
                        activities.Add(activity);
                    }
                }
            }
            return View("ActivityView", activities);
        }


        //View all activities 
        [HttpGet]
        public ActionResult ActivityView()
        {
            List<Activity> activities = GetAllActivities();
            return View(activities);
        }


        public List<Models.Activity> GetAllActivities()
        {
            List<Models.Activity> activities = new List<Models.Activity>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string selectQuery = "SELECT * FROM Activities";

                using (SqlCommand command = new SqlCommand(selectQuery, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Models.Activity activity = new Models.Activity();

                        activity.ActivityId = (int)reader["ActivityId"];
                        activity.ActivityName = (string)reader["ActivityName"];
                        activity.ActivityType = (string)reader["ActivityType"];
                        activity.ActivityDescription = (string)reader["ActivityDescription"];
                        activity.Price = (decimal)reader["Price"];

                        activities.Add(activity);
                    }
                }
            }
            return activities;
        }

        // DELETE ACTIVITY
        [HttpPost]
        public ActionResult DeleteActivity(int activityId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string deleteQuery = "DELETE FROM Activities WHERE ActivityId = @ActivityId";
                using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@ActivityId", activityId);
                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("ActivityView");
        }
    }
}