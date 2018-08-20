using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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

        public static void AddPagination(this HttpResponse response, 
            int currentPage, int itemsPerPage, int totalItems, int totalPages)
        {
            var camelCaseFormatter = new JsonSerializerSettings();
            var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);
            camelCaseFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();
            response.Headers.Add("Pagination", JsonConvert.SerializeObject(paginationHeader, camelCaseFormatter));
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");

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