$(function () {
    setMenuActive('task');

    var LOGSVUE = new Vue({
        el: '#logView',
        data: {
            columns: [],
            logs: [],
            total: 0,
            nodeId: dsApp.getFilter('nodeId') || '',
            identity: dsApp.getFilter('identity') || '',
            size: dsApp.queryString('size') || 40,
            page: dsApp.queryString('page') || 1,
            logType: dsApp.getFilter('logType') || 'All',
            taskName: decodeURIComponent(dsApp.getFilter('taskName') || '')
        },
        mounted: function () {
            loadLogs(this);
        },
        methods: {
            searchResults: function (type) {
                var url = 'log?filter=identity::' + this.$data.identity
                    + '|nodeId::' + this.$data.nodeId
                    + '|logType::' + type
                    + '|taskName::' + this.$data.taskName
                    + '|start::' + ($("#startDate").val() || '')
                    + '|end::' + ($("#endDate").val() || '')
                    + '&page=' + this.$data.page
                    + '&size=' + this.$data.size;
                window.location.href = url;
            }
        }
    });

    function loadLogs(vue) {
        var url = 'api/v1.0/log?filter=identity::' + vue.$data.identity
            + '|nodeId::' + vue.$data.nodeId
            + '|logType::' + vue.$data.logType
            + '|taskName::' + vue.$data.taskName
            + '|start::' + ($("#startDate").val() || '')
            + '|end::' + ($("#endDate").val() || '')
            + '&page=' + vue.$data.page
            + '&size=' + vue.$data.size;
        dsApp.get(url, function (result) {
            vue.$data.columns = result.data.columns;
            vue.$data.logs = result.data.values;
            vue.$data.total = result.data.total;

            dsApp.ui.initPagination('#pagination', result.data, function (page) {
                var url = 'log?filter=identity::' + vue.$data.identity
                    + '|nodeId::' + vue.$data.nodeId
                    + '|logType::' + vue.$data.logType
                    + '|taskName::' + vue.$data.taskName
                    + '|start::' + ($("#startDate").val() || '')
                    + '|end::' + ($("#endDate").val() || '')
                    + '&page=' + page
                    + '&size=' + vue.$data.size;
                window.location.href = url;
            });
        });
    }
});