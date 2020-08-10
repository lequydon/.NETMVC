$(document).ready(function () {
    CreatePickerStart();
    CreatePickerEnd();
    CreateMap();
    GetData();
})
var testFlag = 0;
var shapTemp = null;
var country = "Remain";
//var nameCountry = "Remain";
var startTime = "January 2018";
var EndTime = "August 2018";
//Get_data_with_ajax
function GetData() {
    var nameCountry = "";
    if (country != "Remain")
        nameCountry = country;
    //Statistic order
    $.ajax({
        url: '/DashBoard/ListStatisticalOrder',
        type: "post",
        data: {
            timeStart: startTime,
            timeEnd: EndTime,
            country: nameCountry
        },
        success: function (data) {
            if (data.length == 0) {
                CreateSparklineCustomers();
                CreateSparklineOrders();
                CreateSparklineRevenues();
            }
            else {
                var arraydate = [];
                var arrayCuontCustomer = [];
                var arraySumUnitPrice = [];
                var arrayCountOrder = [];
                for (i in data) {
                    arraydate.push(DateStampToDateTime(data[i].ShippedDate));
                    arrayCuontCustomer.push(data[i].CountCustomer);
                    arraySumUnitPrice.push(data[i].SumUnitPrice);
                    arrayCountOrder.push(data[i].CountOrder);
                    CreateSparklineCustomers(arraydate, arrayCuontCustomer);
                    CreateSparklineOrders(arraydate, arrayCountOrder);
                    CreateSparklineRevenues(arraydate, arraySumUnitPrice);

                }
            }
        }
    })
    //Total Statistical Order
    $.ajax({
        url: '/DashBoard/TotalStatisticalOrder',
        type: "post",
        data: {
            timeStart: startTime,
            timeEnd: EndTime,
            country: nameCountry
        },
        success: function (data) {
            $('#hum_log_revenues').text(kendo.toString(data.UnitPrice, "c2"));
            $('#hum_log_customers').text(data.CountCustomer);
            $('#hum_log_orders').text(data.CountOrder);
        }
    })
    //Country UnitPrice
    $.ajax({
        url: '/DashBoard/CountryUnitPrice',
        type: "post",
        data: {
            timeStart: startTime,
            timeEnd: EndTime,
            country: nameCountry
        },
        success: function (data) {
            if (data.AllCountryUnitPrice != 0)
                $("#donut_market").text((data.CountryUnitprice / data.AllCountryUnitPrice * 100).toFixed(2) + "%")
            else
                $("#donut_market").text((0 + "%"));
            CreateDonutChart(data.CountryUnitprice, data.AllCountryUnitPrice);
        }
    })
}
//
function DateStampToDateTime(value) {
    if (value == "null")
        return;
    var pattern = /Date\(([^)]+)\)/;
    var results = pattern.exec(value);
    var dt = new Date(parseFloat(results[1]));
    return dt.getDate()+"/"+(dt.getMonth() + 1) + "/" + dt.getFullYear();
}
function CreatePickerStart() {
    $("#monthpicker_start").kendoDatePicker({
        //defines the start view
        start: "year",

        // defines when the calendar should return date
        depth: "year",

        // display month and year in the input
        format: "MMMM yyyy",

        // specifies that DateInput is used for masking the input element
        dateInput: true,
        change:GetStartTime
    });
}
function GetStartTime() {
    startTime = kendo.toString(this.value(), 'y');
    GetData();
    //console.log(kendo.toString(this.value(), 'y'));
}
function CreatePickerEnd() {
    $("#monthpicker_end").kendoDatePicker({
        //defines the start view
        start: "year",

        // defines when the calendar should return date
        depth: "year",

        // display month and year in the input
        format: "MMMM yyyy",

        // specifies that DateInput is used for masking the input element
        dateInput: true,
        change: GetEndTime
    });
}
function GetEndTime() {
    EndTime = kendo.toString(this.value(), 'y');
    GetData();
    //console.log(kendo.toString(this.value(), 'y'));
}
function CreateSparklineCustomers(arrayDate, arrayData) {
    $("#hum-log-customers").kendoSparkline({
        theme: "default-v2",
        type: "column",
        data: arrayData,
        categoryAxis: {
            categories: arrayDate
        },
        tooltip: {
            format: "{0}"
        }
    });
}
function CreateSparklineRevenues(arrayDate, arrayData) {
        $("#hum-log-revenues").kendoSparkline({
        theme: "default-v2",
        type: "column",
            data: arrayData,
        categoryAxis: {
            categories: arrayDate
        },
        tooltip: {
            format: "{0:c}"
        }
    });
}
function CreateSparklineOrders(arrayDate, arrayData) {
    $("#hum-log-orders").kendoSparkline({
        theme: "default-v2",
        type: "column",
        data: arrayData,
        categoryAxis: {
            categories: arrayDate
        },
        tooltip: {
            format: "{0}"
        }
    });
}
function CreateDonutChart(datacountry,dataAllCountry) {
    $("#donut_chart_market").kendoChart({
        theme: "flat",
        title: {
            position: "bottom"
        },
        legend: {
            visible: false
        },
        chartArea: {
            background: ""
        },
        seriesDefaults: {
            type: "donut",
            startAngle: 150
        },
        series: [{
            data: [{
                category: country,
                value: datacountry
            }, {
                category: "Remain",
                    value: dataAllCountry
            }]
        }],
        tooltip: {
            visible: true,
            template: "#= category #: #=kendo.toString(value,'c2')#"
        }
    });
}
function CreateMap() {
    $(".map").kendoMap({
        center: [50.000, 0],
        zoom: 2,
        layers: [{
            style: {
                fill: {
                    color: "#1997E4"
                },
                stroke: {
                    color: "#FFFFFF"
                }
            },
            type: "shape",
            dataSource: {
                type: "geojson",
                transport: {
                    read: {
                        dataType: "json",
                        url: "https://raw.githubusercontent.com/telerik/kendoui-northwind-dashboard/master/html/Content/dataviz/map/countries-users.geo.json"
                    }
                }
            }
        }],
        shapeFeatureCreated: OnShapeFeatureCreated,
        shapeMouseEnter: OnShapeMouseEnter,
        shapeMouseLeave: OnShapeMouseLeave,
        shapeClick: OnShapeClick,
        click: OnClick
    });
}
function OnClick() {
    if (testFlag == 1) {
        testFlag = 0;
    }
    else {
        if (shapTemp != null) {
            shapTemp.shape.options.set("fill.opacity", 1);
            shapTemp = null;
        } else
            shapTemp = null;
        country = "Remain";

        //nameCountry = country;
    }
    GetData();
    //shape_flag
    //ajax_call();
}
function OnShapeClick(e) {
    if (e.shape.dataItem.properties.name == "United States of America")
        country = "USA";
    else
        if (e.shape.dataItem.properties.name == "United Kingdom")
            country = "UK";
        else
            country = e.shape.dataItem.properties.name;
    //nameCountry = country;
    testFlag = 1;
    //shap_click
    if (shapTemp != null) {
        shapTemp.shape.options.set("fill.opacity", 1);
        shapTemp = e;
    }
    else
        shapTemp = e;
    //ajax_call();
}
function OnShapeFeatureCreated(e) {
    e.group.options.tooltip = {
        content: e.properties.name,
        position: "cursor",
        offset: 10,
        width: 80
    };
}
function OnShapeMouseEnter(e) {

    e.shape.options.set("fill.opacity", 0.7);
}
function OnShapeMouseLeave(e) {
    if (shapTemp != null)
        shapTemp.shape.options.set("fill.opacity", 0.7);
    if (shapTemp != null) {
        if (shapTemp.shape.dataItem.properties.name != e.shape.dataItem.properties.name) {
            e.shape.options.set("fill.opacity", 1);
        }
    }
    else
        e.shape.options.set("fill.opacity", 1);
}