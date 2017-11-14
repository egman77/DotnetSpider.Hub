$(function () {
    setMenuActive('task');

    var taskId = queryString("TaskId");

    var taskHistoryVue = new Vue({
        el: '#taskHistory',
        data: {
            histories: [],
            total: 0,
            size: 10,
            keyword: '',
            page: 1,
            taskId: 0,
            task:{},
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
            this.$data.taskId = taskId;
            loadTask(this);
            loadHistories(this);
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

    function loadTask(vue) {
    	var url = '/Task/Get';

        dsApp.post(url, { taskId: vue.$data.taskId }, function (result) {
            vue.$data.task = result.result;
    	});
    }

    function loadHistories(vue) {
        var url = '/TaskHistory/Query';
        var keywrod = vue.$data.keyword || '';
        var query = { taskId: vue.$data.taskId, page: vue.$data.page, size: vue.$data.size, keyword: keywrod };

        dsApp.post(url, query, function (result) {
            vue.$data.histories = result.result.result;
            vue.$data.total = result.result.total;

            dsApp.ui.initPagination('#pagination', result.result, function (page) {
                vue.$data.page = page;
                loadHistorys();
            });
        });
    }
});