using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace sqeudulerApp.Controllers
{
    public class TeamController : Controller
    {
        string strCon = "Server=tcp:squeduler.database.windows.net,1433;Initial Catalog=squeduler;Persist Security Info=False;User ID=user;Password=squeduler#123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";


        public IActionResult MainPage()
        {
            return View();
        }

        public IActionResult PersonalPage()
        {
            return View();
        }

        public IActionResult SchedulePage()
        {
            return View();
        }

        // t is de unique code of the team
        public IActionResult TeamInfoPage(string t)
        {
            string teamcode = t;
            
            using SqlConnection conn = new SqlConnection(strCon);
            {
                //sql query. The result  in order is TeamName, City, Code, Address, ZipCode, Owner
                //note: I use parameters for security reasons
                string teamquery = "SELECT [Teams].[Teamname], [Teams].[TeamCity], [Teams].[TeamCode], [Teams].[TeamAddress]," +
                    "[Teams].[TeamZipCode], [Teams].[TeamOwner]" +
                    "FROM [dbo].[Teams] " +
                    "WHERE [Teams].[TeamCode]= @TeamCode;";

                string membersquery = "SELECT [User].[FirstName], [User].[LastName], [UserTeam].[Role], [User].[PhoneNr]," +
                    "[User].[Email]" +
                    "FROM [dbo].[UserTeam] " +
                    "JOIN [dbo].[User] ON [UserTeam].[UserID] = [User].[UserId]" +
                    "WHERE [UserTeam].[Team]= @TeamCode;";

                string rolequery = "SELECT [UserTeam].[Role] " +
                    "FROM [dbo].[UserTeam] " +
                    "JOIN [dbo]. [User] ON [UserTeam].[UserID] = [User].[UserId]" +
                    "WHERE [UserTeam].[Team]= @TeamCode AND [User]. [Email] = @useremail";

                // Create a new list that will contain the 'team information' aka results of the teamquery
                List<string> teaminfo = new List<string>();

                //create a sql command with the team sql query and the original connection string
                using SqlCommand teamcomm = new SqlCommand(teamquery, conn);
                {
                    //here you can give the parameters
                    teamcomm.Parameters.Add("@TeamCode", System.Data.SqlDbType.VarChar);
                    teamcomm.Parameters["@TeamCode"].Value = teamcode;

                    //open the connection
                    conn.Open();

                    //use the original sql datareader and execute the new sql command
                    SqlDataReader sqlResultReader = teamcomm.ExecuteReader();

                    // Iterate through the results of the query a row per itteration
                    while (sqlResultReader.Read())
                    {
                        // for each column in the current row (there should only be one row) add the column info which is in this case
                        // 0. TeamName, 1. City, 2. Code, 3. Address, 4. ZipCode, 5. Owner in that exact order
                        for (int i = 0; i < sqlResultReader.FieldCount; i++)
                        {
                            teaminfo.Add(sqlResultReader.GetValue(i).ToString());
                        }
                    }

                    //close sql reader
                    sqlResultReader.Close();
                    //close sql connection
                    conn.Close();


                }

                // Create a new list that will contain the members aka the results of the membersquery
                List<List<string>> teammembers = new List<List<string>>();

                //create a sql command with the new sql query and the original connection string
                using SqlCommand memberscomm = new SqlCommand(membersquery, conn);
                {
                    //here you can give the parameters
                    memberscomm.Parameters.Add("@TeamCode", System.Data.SqlDbType.VarChar);
                    memberscomm.Parameters["@TeamCode"].Value = teamcode;

                    //open the connection
                    conn.Open();

                    //use the original sql datareader and execute the new sql command
                    SqlDataReader sqlResultReader = memberscomm.ExecuteReader();

                    // Iterate through the results of the query a row per itteration
                    while (sqlResultReader.Read())
                    {
                        List<string> member = new List<string>();

                        // for each column in the current row (there should only be one row) add the column info which is in this case
                        // 0. TeamName, 1. City, 2. Code, 3. Address, 4. ZipCode, 5. Owner in that exact order
                        for (int i = 0; i < sqlResultReader.FieldCount; i++)
                        {
                            member.Add(sqlResultReader.GetValue(i).ToString());
                        }

                        teammembers.Add(member);
                    }

                    //close sql reader
                    sqlResultReader.Close();
                    //close sql connection
                    conn.Close();
                }

                // Create a new list that will contain the members aka the results of the membersquery
                string userrole = null;

                //create a sql command with the new sql query and the original connection string
                using SqlCommand rolecomm = new SqlCommand(rolequery, conn);
                {
                    //here you can give the parameters
                    rolecomm.Parameters.Add("@TeamCode", System.Data.SqlDbType.VarChar);
                    rolecomm.Parameters["@TeamCode"].Value = teamcode;
                    rolecomm.Parameters.Add("@useremail", System.Data.SqlDbType.VarChar);
                    rolecomm.Parameters["@useremail"].Value = HttpContext.Session.GetString("Uid");

                    //open the connection
                    conn.Open();

                    //use the original sql datareader and execute the new sql command
                    SqlDataReader sqlResultReader = rolecomm.ExecuteReader();

                    // Iterate through the results of the query a row per itteration
                    while (sqlResultReader.Read())
                    {
                            userrole = sqlResultReader[0].ToString();
                    }

                    //close sql reader
                    sqlResultReader.Close();
                    //close sql connection
                    conn.Close();
                }

                // create a team context tuple which contains 1. the team information and 2. the members of the team including their information
                Tuple<List<string>, List<List<string>>, string> teamcontext = new Tuple<List<string>, List<List<string>>, string>(teaminfo, teammembers, userrole);

                // add the list to the viewbag dictionary which we can refer to in our html code
                ViewBag.teamcontext = teamcontext;
            }
            return View();
        }
    }
}