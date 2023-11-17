namespace ApiCos.LoginAuthorization
{
    public static class LoginAuthorization
    {
        static Dictionary<string, Dictionary<string,DateTime>> loginAuthorization = new Dictionary<string, Dictionary<string, DateTime>>();

        public static string addAuthorization(string email)
        {
            if(!(loginAuthorization.TryGetValue(email, out Dictionary<string, DateTime> value)))
            {
                loginAuthorization.Add(email, new Dictionary<string, DateTime>());
            }

            string token = Guid.NewGuid().ToString();
            loginAuthorization[email].Add(token, DateTime.Now.AddDays(1));   
            return token;
        }

        public static bool checkAuthorization(string email, string token)
        {
            if(loginAuthorization.TryGetValue(email, out Dictionary<string, DateTime> value))
            {
                if(value.TryGetValue(token, out DateTime date))
                {
                    if(date > DateTime.Now)
                    {
                        return true;
                    }
                    else
                    {
                        value.Remove(token);
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }            
        }   
    }
}
