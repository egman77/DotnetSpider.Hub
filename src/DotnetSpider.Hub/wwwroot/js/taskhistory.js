$(function () {
    var view = new Vue({
        el: '#view',
        data: {
            histories: [],
            total: 0,
            size: hub.queryString('size') || 5,
            page: hub.queryString('page') || 1,
            taskId: hub.queryString("taskId") || '',
            taskName: decodeURIComponent(hub.queryString('taskName') || '')
        },
        mounted: function () {
            var that = this;
            var url = 'api/v1.0/taskhistory?taskId=' + that.$data.taskId + '&page=' + that.$data.page + '&size=' + that.$data.size;
            hub.get(url, function (result) {
                that.$data.histories = result.data.result;
                that.$data.total = result.data.total;

                hub.ui.initPagination('#pagination', result.data, function (page) {
                    window.location.href = 'taskhistory?taskId=' + that.$data.taskId + '&page=' + page + '&size=' + that.$data.size;
                });
            });
        }
    });
});