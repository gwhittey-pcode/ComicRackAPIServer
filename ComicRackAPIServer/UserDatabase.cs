using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace BCR
{
    public class UserDatabase
    {        
        private static Dictionary<string, BCRUser> users = new Dictionary<string, BCRUser>();
        private static object lockObject = new object();

        static UserDatabase()
        {
        }

        // TODO: add timeout for apikey.
        public static IUserIdentity GetUserFromApiKey(string apiKey)
        {
          if (apiKey == null)
            return null;
          
          BCRUser user;
          if (users.TryGetValue(apiKey, out user))
            return user;
          
          NameValueCollection result = Database.Instance.QuerySingle("SELECT user.id as id, username, apikey, fullname FROM user JOIN user_apikeys ON user.id=user_apikeys.user_id WHERE apikey = '" + apiKey + "' LIMIT 1;");
          if (result == null)
            return null;
                    
          user = new BCRUser { UserName = result["username"], UserId = Convert.ToInt32(result["id"]), FullName = result["fullname"] };
          user.Initialize();

          lock (lockObject)
          {
            users[apiKey] = user;
          }

          return user;
        }

        public static string LoginUser(string username, string password)
        {
          NameValueCollection result = Database.Instance.QuerySingle("SELECT * FROM user WHERE username = '" + username + "' COLLATE NOCASE LIMIT 1;");
          if (result == null)
            return null;
          
          SaltedHash.SaltedHash sh = new SaltedHash.SaltedHash();
          if (!sh.VerifyHashString(password, result["password"], result["salt"]))
          {
            // invalid password
            Console.WriteLine("Invalid password for user " + username);
            return null;
          }
          
          //now that the user is validated, create an api key that can be used for subsequent requests
          var apiKey = Guid.NewGuid().ToString();
          
          Database.Instance.ExecuteNonQuery("INSERT INTO user_apikeys (user_id, apikey) VALUES (" + result["id"] + ", '" + apiKey + "');");
          
          return apiKey;            
        }

        public static void RemoveApiKey(string apiKey)
        {
          Database.Instance.ExecuteNonQuery("DELETE FROM user_apikeys WHERE apikey = '" + apiKey + "';");
          users.Remove(apiKey);
        }
        
        public static int GetUserId(string username)
        {
          object result = Database.Instance.ExecuteScalar("SELECT id FROM user WHERE username = '" + username + "' LIMIT 1;");
          if (result == null)
            return -1;          
          else
            return Convert.ToInt32(result);
        }
        
        public static bool AddUser(string username, string password)
        {
          SaltedHash.SaltedHash sh = new SaltedHash.SaltedHash();

          string hash;
          string salt;

          sh.GetHashAndSaltString(password, out hash, out salt);
          
          int result = Database.Instance.ExecuteNonQuery("INSERT INTO user (username, password, salt) VALUES ('" + username + "','" + hash + "','" + salt + "');");
                    
          if (result > 0)
          {
            /*
      ComicListItemFolder userFolder = new ComicListItemFolder(name);
      ComicIdListItem readingList = new ComicIdListItem("Reading");
      userFolder.Items.Add(readingList);
      ComicIdListItem favoritesList = new ComicIdListItem("Favorites");
      userFolder.Items.Add(favoritesList);
            
      ((ComicLibrary)Program.Database).ComicLists.Add(userFolder);
      */
     
            return true;
          }
          
          return false;
        }
        
        public static bool RemoveUser(int userid)
        {
          int result = Database.Instance.ExecuteNonQuery("DELETE FROM user WHERE id = " + userid + ";");
          return result > 0;
        }
        
        public static bool SetPassword(int userid, string password)
        {
          // TODO: validate password strength
          // TODO: remove active api keys

          SaltedHash.SaltedHash sh = new SaltedHash.SaltedHash();

          string hash;
          string salt;

          sh.GetHashAndSaltString(password, out hash, out salt);
          
          int result = Database.Instance.ExecuteNonQuery("UPDATE user SET password='" + hash + "', salt='" + salt + "' WHERE id=" + userid + ";");
          
          return result > 0;
        }
        
        /*
        public static bool SetUsername(int userid, string username)
        {
          // TODO: check if username is unique
          int result = Database.Instance.ExecuteNonQuery("UPDATE user SET username='" + username + "' WHERE id=" + userid + ";");
          
          return result > 0;
        }
        */
        
        public static bool SetFullName(int userid, string fullname)
        {
          int result = Database.Instance.ExecuteNonQuery("UPDATE user SET fullname='" + fullname + "' WHERE id=" + userid + ";");
          
          return result > 0;
        }
        
        
    }
}