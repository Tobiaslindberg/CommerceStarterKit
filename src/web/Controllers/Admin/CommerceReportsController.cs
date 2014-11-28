/*
Commerce Starter Kit for EPiServer

All rights reserved. See LICENSE.txt in project root.

Copyright (C) 2013-2014 Oxx AS
Copyright (C) 2013-2014 BV Network AS

*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using EPiServer.Shell.Navigation;

namespace OxxCommerceStarterKit.Web.Controllers.Admin
{
    [MenuProvider]    
    public class CommerceReportsController : Controller, IMenuProvider
    {
        public IEnumerable<MenuItem> GetMenuItems()
        {
           SectionMenuItem sectionMenuItem = new SectionMenuItem("Commerce Reports", "/global/commercereports");
           sectionMenuItem.IsAvailable = ((RequestContext request) => true);
           UrlMenuItem urlMenuItem = new UrlMenuItem("Latest sales", "/global/commercereports/sales",
              "/commercereports/index");
           urlMenuItem.IsAvailable = ((RequestContext request) => true);
           urlMenuItem.SortIndex = 100;
           UrlMenuItem latestorders = new UrlMenuItem("Latest orders", "/global/commercereports/latestorders",
            "/commercereports/latestorders");
           latestorders.IsAvailable = ((RequestContext request) => true);
           latestorders.SortIndex = 200;


            return new MenuItem[]
            {
                sectionMenuItem ,
                urlMenuItem,
                latestorders				
            };
       }

        [MenuItem("/global/commercereports/index", Text = "Latest sales")]
        public ActionResult Index()
        {
            Dictionary<string,int> values = new Dictionary<string, int>();

            var str = new StringBuilder();

            List<SalesObject> sales = new List<SalesObject>();

            sales.Add(new SalesObject(){Date = new DateTime(2014,9,1),TotalNok= 220, TotalSek = 134});
            sales.Add(new SalesObject() { Date = new DateTime(2014, 9, 2), TotalNok = 420, TotalSek = 164 });
            sales.Add(new SalesObject() { Date = new DateTime(2014, 9, 3), TotalNok = 390, TotalSek = 178 });
            sales.Add(new SalesObject() { Date = new DateTime(2014, 9, 4), TotalNok = 720, TotalSek = 180 });
            sales.Add(new SalesObject() { Date = new DateTime(2014, 9, 5), TotalNok = 620, TotalSek = 395 });
            sales.Add(new SalesObject() { Date = new DateTime(2014, 9, 6), TotalNok = 120, TotalSek = 148 });
            sales.Add(new SalesObject() { Date = new DateTime(2014, 9, 7), TotalNok = 435, TotalSek = 684 });


            str.Append(@"<script type=text/javascript> google.load( ""visualization"", ""1"", {packages:[""corechart""]});
                       google.setOnLoadCallback(drawChart);
                       function drawChart() {
        var data = new google.visualization.DataTable();
        data.addColumn('string', 'Date');
        data.addColumn('number', 'Total NOK (i 1 000)');
        data.addColumn('number', 'Total SEK (i 1 000)');      
 
        data.addRows(" + sales.Count + ");");
 
            for (int i = 0; i <= sales.Count - 1; i++)
            {
                str.Append("data.setValue( " + i + "," + 0 + "," + "'" + sales[i].Date.ToString("dd.MM") + "');");
                str.Append("data.setValue(" + i + "," + 1 + "," + sales[i].TotalNok.ToString() + ") ;");
                str.Append("data.setValue(" + i + "," + 2 + "," + sales[i].TotalSek.ToString() + ") ;");
            }
 
            str.Append(" var chart = new google.visualization.ColumnChart(document.getElementById('chart_div'));");
            str.Append(" chart.draw(data, {width: 650, height: 300, title: 'Sales latest 5 days',");
            str.Append("hAxis: {title: 'Date', titleTextStyle: {color: 'green'}}");
            str.Append("}); }");
            str.Append("</script>");
            

            return View("Index",str);
        }


        [MenuItem("/global/commercereports/latestorders", Text = "Latest orders")]
        public ActionResult LatestOrders()
        {          

            var str = new StringBuilder();

            List<OrderObject> orders = new List<OrderObject>();

            orders.Add(new OrderObject() { Total = 1220});
            orders.Add(new OrderObject() { Total = 1420});
            orders.Add(new OrderObject() { Total = 1390});
            orders.Add(new OrderObject() { Total = 720});
            orders.Add(new OrderObject() { Total = 1620});
            orders.Add(new OrderObject() { Total = 1120});
            orders.Add(new OrderObject() { Total = 3435});


            str.Append(@"<script type=text/javascript> google.load( ""visualization"", ""1"", {packages:[""corechart""]});
                       google.setOnLoadCallback(drawChart);
                       function drawChart() {
        var data = new google.visualization.DataTable();    
data.addColumn('number', '#');    
        data.addColumn('number', 'Total (i 1 000)');
            
 
        data.addRows(" + orders.Count + ");");

            for (int i = 0; i <= orders.Count - 1; i++)
            {
                str.Append("data.setValue(" + i + "," + 0 + "," + (i+1) + ") ;");                
                str.Append("data.setValue(" + i + "," + 1 + "," + orders[i].Total.ToString() + ") ;");                
            }

            str.Append(" var chart = new google.visualization.ColumnChart(document.getElementById('chart_div'));");
            str.Append(" chart.draw(data, {width: 650, height: 300, title: 'Latest orders',");
            str.Append("hAxis: {title: '#', titleTextStyle: {color: 'green'}}");
            str.Append("}); }");
            str.Append("</script>");


            return View(str);
        }

    }

    public class OrderObject
    {
        public DateTime Date { get; set; }
        public int Total { get; set; }
    }

    public class SalesObject
    {
        public DateTime Date { get; set; }
        public int TotalSek { get; set; }
        public double TotalNok { get; set; }
            
    }
}
