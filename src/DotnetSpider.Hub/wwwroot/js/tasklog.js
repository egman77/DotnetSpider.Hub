$(function () {
    var view = new Vue({
        el: '#view',
        data: {
            logs: [],
            total: 0,
            nodeId: hub.queryString('nodeId') || '',
            identity: hub.queryString('identity') || '',
            size: hub.queryString('size') || 40,
            page: hub.queryString('page') || 1,
            logType: hub.queryString('logType') || 'All',
            taskName: decodeURIComponent(hub.queryString('taskName') || '')
        },
        mounted: function () {
            var that = this;
            var url = 'api/v1.0/tasklog?identity=' + that.$data.identity
                + '&nodeId=' + that.$data.nodeId
                + '&logType=' + that.$data.logType
                + '&taskName=' + that.$data.taskName
                + '&start=' + ($("#start").val() || '')
                + '&end=' + ($("#end").val() || '')
                + '&page=' + that.$data.page
                + '&size=' + that.$data.size;
            hub.get(url, function (result) {
                that.$data.logs = result.data.result;
                that.$data.total = result.data.total;
                that.$data.page = result.data.page;
                that.$data.size = result.data.size;

                hub.ui.initPagination('#pagination', result.data, function (page) {
                    window.location.href = url;
                });
            });

            $('select').formSelect();
        },
        methods: {
            search: function () {
                var url = 'tasklog?identity=' + this.$data.identity
                    + '&logType=' + this.$data.logType
                    + '&taskName=' + this.$data.taskName;
                window.location.href = url;
            }
        }
    });
});