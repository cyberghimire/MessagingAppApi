using System;
using Microsoft.AspNetCore.Http;

namespace MessagingApp.API.Helpers
{
    public static class Extensions
    {
        public static void AddApplicationError(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }

        public static int CalculateAge(this DateTime dateTime)
        {
            var age = DateTime.Today.Year - dateTime.Year;
            if(dateTime.AddYears(age) > DateTime.Today)         //if the user hasn't had their birthday yet this year, then we need to subtract 1 year off the 'Age'. 
                age--;
            
            return age;
        }
        
    }
}