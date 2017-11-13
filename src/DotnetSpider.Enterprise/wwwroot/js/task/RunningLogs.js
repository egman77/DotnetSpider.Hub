$(function () {
    var node = queryString('node');
    var identity = queryString('identity');
    var refresh = queryString('refresh')=='1';
    var lastQuery;

    var tasksVUE = new Vue({
        el: '#taskContent',
        data: {
            logs: [],
            logsPaging: {
                total: 0,
                size: 10,
                keyword: '',
                page: 1
            }
        },
        mounted: function () {
            var that = this;
            $("#logType").val('');
            that.logsPaging.total = 0;
            that.logsPaging.page = 1;
            lastQuery = null;
            loadBatchLogs(that);
        },
        methods: {
            searchResults: function () {
                var that = this;
                that.logsPaging.total = 0;
                that.logsPaging.page = 1;
                lastQuery = null;
                loadBatchLogs();
            }
        }
    });

    function queryString(name) {
        var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
        if (result == null || result.length < 1) {
            return "";
        }
        return result[1];
    }

    function loadBatchLogs(vue) {
        var logVue = tasksVUE || vue;
        var query;
        if (!lastQuery) {
            query = {
                identity: identity,
                node: node,
                startDate: $("#startDate").val(),
                endDate: $("#endDate").val(),
                page: logVue.$data.logsPaging.page,
                size: logVue.$data.logsPaging.size,
                logType: $("#logType").val(),
            };
            lastQuery = query;
        }
        else query = lastQuery;

        dsApp.post('/task/getBatchLogs', query, function (result) {
            logVue.$data.logs = result.result.result;
            logVue.$data.logsPaging.total = result.result.total;

            dsApp.ui.initPagination('#logsPagination', result.result, function (page) {
                logVue.$data.logsPaging.page = page;
                if (lastQuery) lastQuery.page = page;
                loadBatchLogs();
            });
        });
    }

    setTimeout(function () {
        $(".menu li").removeClass("active");
        $(".menu li a").removeClass("toggled");
        $(".menu li#taskState").addClass("active");
        $(".menu li#taskState a").addClass("toggled");
    }, 50);
});