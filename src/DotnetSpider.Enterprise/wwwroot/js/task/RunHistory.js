$(function () {
    var proj = queryString("taskId");

    var batchVue = new Vue({
        el: '#taskContent',
        data: {
            historys: [],
            total: 0,
            size: 10,
            keyword: '',
            page: 1,
            taskId: 0,
            logs: [],
            logsPaging: {
                total: 0,
                size: 10,
                keyword: '',
                page: 1,
                identity: '',
                node:''
            }
        },
        mounted: function () {
            this.$data.page = 1;
            this.$data.taskId = proj;
            loadHistorys(this);
        },
        methods: {
            viewLogs: function (batchId, nodeId) {
                var that = this;
                $("#logType").val('');
                that.logsPaging.total = 0;
                that.logsPaging.page = 1;
                that.logsPaging.identity = batchId;
                that.logsPaging.node = nodeId;
                lastQuery = null;
                $("#LogsModal").modal("show");
                loadBatchLogs();
            },
            searchResults: function () {
                var that = this;
                that.logsPaging.total = 0;
                that.logsPaging.page = 1;
                lastQuery = null;
                loadBatchLogs();
            },
            viewBatchLogs: function (batchId) {
                var that = this;
                $("#logType").val('');
                that.logsPaging.total = 0;
                that.logsPaging.page = 1;
                that.logsPaging.identity = batchId;
                that.logsPaging.node = '';
                lastQuery = null;
                $("#LogsModal").modal("show");
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

    var lastQuery;

    function loadBatchLogs() {
        var query;
        if (!lastQuery) {
            query = {
                identity: batchVue.$data.logsPaging.identity,
                node: batchVue.$data.logsPaging.node,
                startDate: $("#startDate").val(),
                endDate: $("#endDate").val(),
                page: batchVue.$data.logsPaging.page,
                size: batchVue.$data.logsPaging.size,
                logType: $("#logType").val(),
            };
            lastQuery = query;
        }
        else query = lastQuery;

        dsApp.post('/task/getBatchLogs', query, function (result) {
            batchVue.$data.logs = result.result.result;
            batchVue.$data.logsPaging.total = result.result.total;

            dsApp.ui.initPagination('#logsPagination', result.result, function (page) {
                batchVue.$data.logsPaging.page = page;
                if (lastQuery) lastQuery.page = page;
                loadBatchLogs();
            });
        });
    }

    function loadHistorys(batchVue) {
    	var url = '/Task/QueryRunHistory';
        var keywrod = batchVue.$data.keyword || '';
        var query = { taskId: batchVue.$data.taskId, page: batchVue.$data.page, size: batchVue.$data.size, keyword: keywrod };

        dsApp.post(url, query, function (result) {
        	batchVue.$data.historys = result.result.result;
            batchVue.$data.total = result.result.total;
            //batchVue.$data.solutions = result.projects;

            dsApp.ui.initPagination('#pagination', result.result, function (page) {
                batchVue.$data.page = page;
                loadHistorys();
            });
        });
    }

    setTimeout(function () {
        $(".menu li").removeClass("active");
        $(".menu li a").removeClass("toggled");
        $(".menu li#tasks").addClass("active");
        $(".menu li#tasks a").addClass("toggled");
    }, 50);
});