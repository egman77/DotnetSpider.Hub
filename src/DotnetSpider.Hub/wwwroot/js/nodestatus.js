$(function () {
    setMenuActive("nodes");
    nodeDashboardVue = new Vue({
        el: '#dashboardBody',
        data: {
            nodeDetail: {},
            logs: [],
            total: 0,
            size: 20,
            logLevel: '',
            currentPage: 1
        },
        mounted: function () {
            loadNodeDetail(this);
            loadNodeLogs(this);
            $("a[level='']").parent().addClass('active');
        },
        methods: {
        }
    });

    function loadNodeDetail(vue) {
        var agentId = $("#AgentId").val();
        var input = { id: agentId };
        var url = '/Node/GetNodeDetail';
        dsApp.post(url, input, function (result) {
            vue.$data.nodeDetail = result.result;
            setTimeout(() => {
                setPerformanceChart(result.result.performanceData);
                setTaskChart(result.result.performanceData);
            });
        });
    }

    function loadNodeLogs(vue) {
        var agentId = $("#AgentId").val();
        var logLevel = vue.$data.logLevel;
        var input = {
            agentId: agentId,
            logLevel: logLevel,
            page: vue.$data.currentPage,
            size: vue.$data.size
        };

        var url = '/Node/GetLog';
        dsApp.post(url, input, function (result) {
            vue.$data.logs = result.result.result;        
            vue.$data.total = result.result.total;

            dsApp.ui.initPagination('#pagination', result.result, function (page) {
                vue.$data.currentPage = page;
                loadNodeLogs(vue);
            });
        });
    }

    $("#enableSwitch").change(function () {
        var isEnable = $(this).prop('checked');
        var agentId = $("#AgentId").val();
        var input = { id: agentId, enable: isEnable };

        var url = '/Node/EnableNode';

        dsApp.ui.setBusy();
        dsApp.post(url, input, function (result) {
            $(this).prop('checked', result.result.enable);
            dsApp.ui.clearBusy();
        });
    });

    function setPerformanceChart(data) {
        $("#performanceChart").html('');
        Morris.Line({
            element: 'performanceChart',
            data: data,
            xkey: 'time',
            ykeys: ['cpu', 'memory'],
            labels: ['CPU Load(%)', 'Memory Load(%)'],
            lineColors: ['rgb(233, 30, 99)', 'rgb(0, 188, 212)'],
            lineWidth: 1,
            pointSize:3
        });
    }

    function setTaskChart(data) {
        $("#taskChart").html('');
        Morris.Line({
            element: 'taskChart',
            data: data,
            xkey: 'time',
            ykeys: ['runningTasks'],
            labels: ['Count of Running Tasks'],
            lineColors: ['rgb(233, 30, 99)'],
            lineWidth: 1,
            pointSize: 3
        });
    }

    $(".logLevel").click(function() {
        nodeDashboardVue.$data.currentPage = 1;
        nodeDashboardVue.$data.logLevel = $(this).attr('level');
        $(".logLevel").each(function() {
            $(this).parent().removeClass("active");
        });
        $(this).parent().addClass("active");
        loadNodeLogs(nodeDashboardVue);
    });

    function time(f, vue, t) {
        return function walk() {
            setTimeout(function () {
                f(vue);
                walk();
            }, t);
        };
    }

    time(loadNodeDetail, nodeDashboardVue, 60000)();
});



