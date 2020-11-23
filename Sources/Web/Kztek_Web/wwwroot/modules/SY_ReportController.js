$(function () {
    $('body').on('click', '.btnSearchMonthly', function () {
        SY_ReportController.Month_Performance();
    });

    $('body').on('click', '.btnSearchTeam', function () {
        SY_ReportController.Team_Performance();
    });

    $('body').on('click', '.btnSearchGrow', function () {
        SY_ReportController.Grow_Performance();
    });
});

var SY_ReportController = {
    init() {

    },
    Month_Performance() {
        var ids = $('#slUser').val();

        JSHelper.AJAX_SendRequest('/SY_Report/Report_PerformanceMonthly_Data', { ItemValue: JSON.stringify(ids) })
            .success(function (response) {
                //Khai tạo phần dữ liệu

                $('#btlReport tbody').html('');

                response.forEach(function (item) {
                    var str = '';

                    str = "<tr>";

                    str += "<td>" + item.Username + "</td>";

                    str += "<td>" + "<canvas id='ChartTask_" + item.UserId + "'></canvas>" + "</td>";
                    str += "<td>" + "<canvas id='ChartProject_" + item.UserId + "'></canvas>" + "</td>";


                    str += "</tr>";

                    $('#btlReport tbody').append(str);
                });

                response.forEach(function (item) {
                    var ctx = document.getElementById('ChartTask_' + item.UserId).getContext('2d');
                    var ctx1 = document.getElementById('ChartProject_' + item.UserId).getContext('2d');

                    //Total
                    var DataLabel = [];

                    var DataProjectTotal = [];
                    var DataProjectCompleted_onTime = [];
                    var DataProjectCompleted_notOnTime = [];

                    var DataTaskTotal = [];
                    var DataTaskCompleted_onTime = [];
                    var DataTaskCompleted_notOnTime = [];

                    item.Details.forEach(function (itemDetail) {
                        DataLabel.push(itemDetail.Month);

                        DataProjectTotal.push(itemDetail.Project_Total);
                        DataProjectCompleted_onTime.push(itemDetail.Project_Completed_onTime);
                        DataProjectCompleted_notOnTime.push(itemDetail.Project_Completed_notOnTime);

                        DataTaskTotal.push(itemDetail.Task_Total);
                        DataTaskCompleted_onTime.push(itemDetail.Task_Completed_onTime);
                        DataTaskCompleted_notOnTime.push(itemDetail.Task_Completed_notOnTime);
                    });

                    var dataProjectTotal = {
                        label: "Tổng công việc",
                        data: DataProjectTotal,
                        lineTension: 0,
                        fill: false,
                        borderColor: 'blue'
                    };

                    var dataProjectComplete_onTime = {
                        label: "Công việc hoàn thành đúng hạn",
                        data: DataProjectCompleted_onTime,
                        lineTension: 0,
                        fill: false,
                        borderColor: 'green'
                    };

                    var dataProjectComplete_notOnTime = {
                        label: "Công việc hoàn thành không đúng hạn",
                        data: DataProjectCompleted_notOnTime,
                        lineTension: 0,
                        fill: false,
                        borderColor: 'red'
                    };


                    var dataTaskTotal = {
                        label: "Tổng công việc",
                        data: DataTaskTotal,
                        lineTension: 0,
                        fill: false,
                        borderColor: 'blue'
                    };

                    var dataTaskComplete_onTime = {
                        label: "Công việc hoàn thành đúng hạn",
                        data: DataTaskCompleted_onTime,
                        lineTension: 0,
                        fill: false,
                        borderColor: 'green'
                    };

                    var dataTaskComplete_notOnTime = {
                        label: "Công việc hoàn thành không đúng hạn",
                        data: DataTaskCompleted_notOnTime,
                        lineTension: 0,
                        fill: false,
                        borderColor: 'red'
                    };

                    var userProjectData = {
                        labels: DataLabel,
                        datasets: [dataProjectTotal, dataProjectComplete_onTime, dataProjectComplete_notOnTime]
                    };

                    var userTaskData = {
                        labels: DataLabel,
                        datasets: [dataTaskTotal, dataTaskComplete_onTime, dataTaskComplete_notOnTime]
                    };

                    var chartOptions = {
                        legend: {
                            display: true,
                            position: 'top',
                            labels: {
                                boxWidth: 80,
                                fontColor: 'black'
                            }
                        }
                    };

                    var lineChart = new Chart(ctx, {
                        type: 'line',
                        data: userTaskData,
                        options: chartOptions
                    });

                    var lineChart1 = new Chart(ctx1, {
                        type: 'line',
                        data: userProjectData,
                        options: chartOptions
                    });
                });

            });
    },
    Team_Performance() {
        var ids = $('#slUser').val();

        JSHelper.AJAX_SendRequest('/SY_Report/Report_PerformanceTeam_Data', { ItemValue: JSON.stringify(ids) })
            .success(function (response) {
                //Khai tạo phần dữ liệu

                $('#btlReport tbody').html('');

                var str = '';

                str = "<tr>";

                str += "<td>" + "<canvas id='Chart_Team'></canvas>" + "</td>";

                str += "</tr>";

                $('#btlReport tbody').append(str);


                var ctx = document.getElementById('Chart_Team').getContext('2d');

                //Total
                var DataUser_Project = [];
                var DataUser_Task = [];
                var DataLabel = [];
                var DataUser = [];

                response.forEach(function (item) {
                    DataLabel.push(item.Username);
                    DataUser_Project.push(item.Percent.Project_Percent);
                    DataUser_Task.push(item.Percent.Task_Percent);

                    DataUser.push([item.Percent.Project_Percent, item.Percent.Task_Percent])
                });

                var data = {
                    labels: DataLabel,
                    datasets: [
                        {
                            label: "Project (%)",
                            backgroundColor: "red",
                            data: DataUser_Project
                        },
                        {
                            label: "Task (%)",
                            backgroundColor: "teal",
                            data: DataUser_Task
                        },
                    ]
                };

                var horizontalBar = new Chart(ctx, {
                    type: 'horizontalBar',
                    data: data,
                    options: {
                        barValueSpacing: 20
                    }
                });

            });
    },
    Grow_Performance() {
        var ids = $('#slUser').val();

        JSHelper.AJAX_SendRequest('/SY_Report/Report_PerformanceGrow_Data', { ItemValue: JSON.stringify(ids) })
            .success(function (response) {
                $('#tblGrowReport tbody').html('');

                response.forEach(function (item) {
                    var str = '';

                    str = "<tr>";

                    str += "<td>" + item.Username + "</td>";
                    str += "<td>" + "<canvas id='ChartTask_" + item.UserId + "'></canvas>" + "</td>";
                    str += "<td>" + "<canvas id='ChartProject_" + item.UserId + "'></canvas>" + "</td>";

                    str += "</tr>";

                    $('#tblGrowReport tbody').append(str);
                });

                response.forEach(function (item) {
                    var ctx = document.getElementById('ChartTask_' + item.UserId).getContext('2d');
                    var ctx1 = document.getElementById('ChartProject_' + item.UserId).getContext('2d');

                    //Total
                    var DataTask_Percent = [];
                    var DataTask_GrowPercent = [];

                    var DataProject_Percent = [];
                    var DataProject_GrowPercent = [];

                    var DataLabel = [];

                    item.Details.forEach(function (itemDetail) {
                        DataLabel.push(itemDetail.Month);

                        DataTask_Percent.push(itemDetail.Task_CurrentPercent);
                        DataTask_GrowPercent.push(itemDetail.Task_GrowPercent);

                        DataProject_Percent.push(itemDetail.Project_CurrentPercent);
                        DataProject_GrowPercent.push(itemDetail.Project_GrowPercent);
                    });

                    var dataPercent = {
                        label: "Hiệu quả",
                        data: DataTask_Percent,
                        lineTension: 0,
                        fill: false,
                        borderColor: 'blue'
                    };

                    var dataGrowPercent = {
                        label: "Tăng tưởng",
                        data: DataTask_GrowPercent,
                        lineTension: 0,
                        fill: false,
                        borderColor: 'pink'
                    };

                    var userData = {
                        labels: DataLabel,
                        datasets: [dataPercent, dataGrowPercent]
                    };

                    var dataProjectPercent = {
                        label: "Hiệu quả",
                        data: DataProject_Percent,
                        lineTension: 0,
                        fill: false,
                        borderColor: 'blue'
                    };

                    var dataProjectGrowPercent = {
                        label: "Tăng tưởng",
                        data: DataProject_GrowPercent,
                        lineTension: 0,
                        fill: false,
                        borderColor: 'pink'
                    };

                    var userProjectData = {
                        labels: DataLabel,
                        datasets: [dataProjectPercent, dataProjectGrowPercent]
                    };

                    var chartOptions = {
                        legend: {
                            display: true,
                            position: 'top',
                            labels: {
                                boxWidth: 80,
                                fontColor: 'black'
                            }
                        }
                    };

                    var lineChart = new Chart(ctx, {
                        type: 'line',
                        data: userData,
                        options: chartOptions
                    });

                    var lineChart1 = new Chart(ctx1, {
                        type: 'line',
                        data: userProjectData,
                        options: chartOptions
                    });

                });
            });
    }
    
};
