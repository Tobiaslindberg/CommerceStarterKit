/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace OxxCommerceStarterKit.Web.Services
{
    public class PostNord
    {

        private const string BusinessLocationServiceUrlSettingsKey = "PostNord.BusinessLocation.ServiceUri";
        private const string BusinessLocationServiceUrlDefaultValue = "http://api.postnord.com/wsp/rest/BusinessLocationLocator/Logistics/ServicePointService_1.0/";
        private const string ConsumerIdSettingsKey = "PostNord.ConsumerId";

        class ServicePointInformationResponseWrapper
        {
            public ServicePointInformationResponse servicePointInformationResponse { get; set; }
        }

        class ServicePointInformationResponse
        {
            public IEnumerable<ServicePointDto> servicePoints { get; set; }
        }

        class ServicePointDto
        {
            public AddressDto visitingAddress { get; set; }
            public AddressDto deliveryAddress { get; set; }
            public CoordinateDto coordinate { get; set; }
            public string handlingOffice { get; set; }
            public string name { get; set; }
            public IEnumerable<OpeningHoursDto> openingHours { get; set; }
            public int routeDistance { get; set; }
            public string routingCode { get; set; }
            public string servicePointId { get; set; }

            public ServicePoint Parse()
            {
                var servicePoint = new ServicePoint() { HandlingOffice = handlingOffice, Name = name, RouteDistance = routeDistance, RoutingCode = routingCode, Id = servicePointId };

                if (visitingAddress != null)
                    servicePoint.VisitingAddress = visitingAddress.Parse();

                if (deliveryAddress != null)
                    servicePoint.DeliveryAddress = deliveryAddress.Parse();

                if (openingHours != null)
                    servicePoint.OpeningHours = openingHours.Select(h => h.Parse());

                return servicePoint;
            }
        }

        class AddressDto
        {
            public string streetName { get; set; }
            public string streetNumber { get; set; }
            public string postalCode { get; set; }
            public string city { get; set; }
            public string countryCode { get; set; }

            public Address Parse()
            {
                var address = new Address() { StreetName = streetName, StreetNumber = streetNumber, PostalCode = postalCode, City = city };

                if (countryCode != null)
                    address.Region = new RegionInfo(countryCode);

                return address;
            }
        }

        class CoordinateDto
        {

            public string countryCode { get; set; }
            public double easting { get; set; }
            public double northing { get; set; }
            public string srId { get; set; }

            public Coordinate Parse()
            {
                var coordinate = new Coordinate() { Latitude = northing, Longitude = easting, SrId = srId };

                if (countryCode != null)
                    coordinate.Region = new RegionInfo(countryCode);

                return coordinate;
            }
        }

        class OpeningHoursDto
        {
            public string day { get; set; }
            public string from1 { get; set; }
            public string from2 { get; set; }
            public string to1 { get; set; }
            public string to2 { get; set; }

            public OpeningHours Parse()
            {
                OpeningHours openingHours = new OpeningHours();

                switch (day)
                {
                    case "MO":
                        openingHours.Day = DayOfWeek.Monday;
                        break;
                    case "TU":
                        openingHours.Day = DayOfWeek.Tuesday;
                        break;
                    case "WE":
                        openingHours.Day = DayOfWeek.Wednesday;
                        break;
                    case "TH":
                        openingHours.Day = DayOfWeek.Thursday;
                        break;
                    case "FR":
                        openingHours.Day = DayOfWeek.Friday;
                        break;
                    case "SA":
                        openingHours.Day = DayOfWeek.Saturday;
                        break;
                    case "SU":
                        openingHours.Day = DayOfWeek.Sunday;
                        break;
                }

                int i;

                if (from1 != null && int.TryParse(from1, out i))
                    openingHours.From1 = new TimeSpan(i / 100, i - i / 100 * 100, 0);

                if (to1 != null && int.TryParse(to1, out i))
                    openingHours.To1 = new TimeSpan(i / 100, i - i / 100 * 100, 0);

                if (from2 != null && int.TryParse(from2, out i))
                    openingHours.From2 = new TimeSpan(i / 100, i - i / 100 * 100, 0);

                if (to2 != null && int.TryParse(to2, out i))
                    openingHours.To2 = new TimeSpan(i / 100, i - i / 100 * 100, 0);

                return openingHours;
            }
        }

        public class ServicePoint
        {
            public Address VisitingAddress { get; set; }
            public Address DeliveryAddress { get; set; }
            public Coordinate Coordinate { get; set; }
            public string HandlingOffice { get; set; }
            public string Name { get; set; }
            public IEnumerable<OpeningHours> OpeningHours { get; set; }
            public int RouteDistance { get; set; }
            public string RoutingCode { get; set; }
            public string Id { get; set; }
        }

        public class Address
        {
            public string StreetName { get; set; }
            public string StreetNumber { get; set; }
            public string PostalCode { get; set; }
            public string City { get; set; }
            public RegionInfo Region { get; set; }
        }

        public class Coordinate
        {

            public double Latitude { get; set; }
            public double Longitude { get; set; }

            /// <summary>
            ///     An EPSG Spatial Reference System Identifier (SRID) is a unique value used to unambiguously identify projected, unprojected, and local spatial coordinate system definitions.Default value is EPSG:4326
            ///     
            ///     The following spatial reference systems shall be supported:
            ///     
            ///     EPSG:3785 - Spherical Mercator – applicable for all countries.
            ///     EPSG:4326 - World Geodetic System / WGS 84, also known as lat/lon – applicable for all countries.
            ///     EPSG:3006 - sweref99tm - official system for Sweden.
            ///     EPSG:25832 – ETRS89 / UTM zone 32N - official system for Denmark.
            ///     EPSG:2393 - KKJ / Finland Uniform Coordinate System.
            ///     EPSG:32633 - WGS 84 / UTM zone 33N – official system for Norway.
            ///     spatialreferencing.org:900913 - Spherical Mercator – applicable for all countries.
            /// </summary>
            public string SrId { get; set; }

            public RegionInfo Region { get; set; }
        }

        public struct OpeningHours
        {

            public DayOfWeek Day { get; set; }
            public TimeSpan From1 { get; set; }
            public TimeSpan To1 { get; set; }
            public TimeSpan From2 { get; set; }
            public TimeSpan To2 { get; set; }
        }

        /// <summary>
        /// Provides a quick way of getting the default service point for a postal code.
        /// </summary>
        public static async Task<ServicePoint> FindByPostalCode(RegionInfo regionInfo, string postalCode)
        {
            using (var client = new HttpClient())
            {
                string serviceUrl = GetSetting(BusinessLocationServiceUrlSettingsKey,
                    BusinessLocationServiceUrlDefaultValue);
                client.BaseAddress = new Uri(serviceUrl);

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync(string.Format("findByPostalCode.json?consumerId={0}&countryCode={1}&postalCode={2}", 
                    HttpUtility.UrlEncode(GetConsumerId()), 
                    HttpUtility.UrlEncode(regionInfo.TwoLetterISORegionName), 
                    HttpUtility.UrlEncode(postalCode)));

                response.EnsureSuccessStatusCode();

                return (await response.Content.ReadAsAsync<ServicePointInformationResponseWrapper>())
                    .servicePointInformationResponse.servicePoints.Select(p => p.Parse()).FirstOrDefault();
            }
        }

        /// <summary>
        /// Provides the nearest service points from an address based on driving route distance
        /// </summary>
        /// <param name="city">Name of the city. Mandatory, if postalCode is not provided.</param>
        /// <param name="postalCode">Postal Code. Mandatory, if city is not provided.</param>
        /// <param name="srId">
        ///     An EPSG Spatial Reference System Identifier (SRID) is a unique value used to unambiguously identify projected, unprojected, and local spatial coordinate system definitions.Default value is EPSG:4326
        ///     
        ///     The following spatial reference systems shall be supported:
        ///     
        ///     EPSG:3785 - Spherical Mercator – applicable for all countries.
        ///     EPSG:4326 - World Geodetic System / WGS 84, also known as lat/lon – applicable for all countries.
        ///     EPSG:3006 - sweref99tm - official system for Sweden.
        ///     EPSG:25832 – ETRS89 / UTM zone 32N - official system for Denmark.
        ///     EPSG:2393 - KKJ / Finland Uniform Coordinate System.
        ///     EPSG:32633 - WGS 84 / UTM zone 33N – official system for Norway.
        ///     spatialreferencing.org:900913 - Spherical Mercator – applicable for all countries.
        /// </param>
        public static async Task<IEnumerable<ServicePoint>> FindNearestByAddress(RegionInfo regionInfo, string city, string postalCode, string streetName = null, string streetNumber = null, int numberOfServicePoints = 5, string srId = "EPSG:4326")
        {
            using (var client = new HttpClient())
            {
                string serviceUrl = GetSetting(BusinessLocationServiceUrlSettingsKey,
                    BusinessLocationServiceUrlDefaultValue);
                client.BaseAddress = new Uri(serviceUrl);

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var address = string.Format("findNearestByAddress.json?consumerId={0}&countryCode={1}", 
                    HttpUtility.UrlEncode(GetConsumerId()), 
                    HttpUtility.UrlEncode(regionInfo.TwoLetterISORegionName));

                if (city != null)
                    address += string.Format("&city={0}", HttpUtility.UrlEncode(city));

                if (postalCode != null)
                    address += string.Format("&postalCode={0}", HttpUtility.UrlEncode(postalCode));

                else if (city == null)
                    throw new ArgumentException("city, postalCode");

                if (streetName != null)
                    address += string.Format("&streetName={0}", HttpUtility.UrlEncode(streetName));

                if (streetNumber != null)
                    address += string.Format("&streetNumber={0}", HttpUtility.UrlEncode(streetNumber));

                if (numberOfServicePoints != 5)
                    address += string.Format("&numberOfServicePoints={0}", HttpUtility.UrlEncode(numberOfServicePoints.ToString()));

                if (srId != null && srId != "EPSG:4326")
                    address += string.Format("&srId={0}", HttpUtility.UrlEncode(srId));

                var response = await client.GetAsync(address);

                response.EnsureSuccessStatusCode();

                return (await response.Content.ReadAsAsync<ServicePointInformationResponseWrapper>())
                    .servicePointInformationResponse.servicePoints.Select(p => p.Parse());
            }
        }

        protected static string GetSetting(string key, string defaultValue)
        {
            string value = ConfigurationManager.AppSettings[key];
            if(value != null)
            {
                return value;
            }
            return defaultValue;
        }

        protected static string GetConsumerId()
        {
            string consumerId = GetSetting(ConsumerIdSettingsKey, null);
            if (consumerId != null)
            {
                return consumerId;
            }
            else
            {
                throw new ConfigurationErrorsException("Missing PostNord.ConsumerId application setting.");
            }

        }
    }
}
