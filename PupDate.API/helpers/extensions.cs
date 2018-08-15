using System;
using Microsoft.AspNetCore.Http;

namespace PupDate.API.helpers
{
    public static class extensions
    {
        public static void AddApplicationError(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }

        public static int CalculateAge(this DateTime theDateTime)
        {
            var age = DateTime.Today.Year - theDateTime.Year;
            // check to see if age is greater than todays date, if true then take a year off age returned
            if (theDateTime.AddYears(age) > DateTime.Today)
                age--;

                return age;
        }
    }
}