$(function () {
    setMenuActive('task');

    var node = queryString('node');
    var identity = queryString('identity');
    var refresh = queryString('refresh') == '1';
    var lastQuery;

    var tasksVUE = new Vue({
        el: '#logView',
        data: {
            columns: [],
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
            loadLogs(that);
        },
        methods: {
            searchResults: function () {
                var that = this;
                that.logsPaging.total = 0;
                that.logsPaging.page = 1;
                lastQuery = null;
                loadLogs();
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

    function loadLogs(vue) {
        var logVue = tasksVUE || vue;
        var query;
        if (!lastQuery) {
            query = {
                identity: identity,
                node: node,
                //startDate: $("#startDate").val(),
                //endDate: $("#endDate").val(),
                page: logVue.$data.logsPaging.page,
                size: logVue.$data.logsPaging.size//,
                //logType: $("#logType").val(),
            };
            lastQuery = query;
        }
        else query = lastQuery;

        dsApp.post('/log/query', query, function (result) {
            logVue.$data.columns = result.result.columns;
            logVue.$data.logs = result.result.values;
            logVue.$data.logsPaging.total = result.result.total;

            dsApp.ui.initPagination('#logsPagination', result.result, function (page) {
                logVue.$data.logsPaging.page = page;
                if (lastQuery) lastQuery.page = page;
                loadLogs();
            });
        });
    }
});