$(function () {
    setMenuActive('task');

    var taskHistoryVue = new Vue({
        el: '#taskHistory',
        data: {
            histories: [],
            total: 0,
            size: dsApp.queryString('size') || 5,
            page: dsApp.queryString('page') || 1,
            taskId: dsApp.getFilter("taskId") || '',
            taskName: decodeURIComponent(dsApp.getFilter('taskName') || '')
        },
        mounted: function () {
            loadHistories(this);
        }
    });

    function loadHistories(vue) {
        var url = 'api/v1.0/taskHistory?filter=taskId::' + vue.$data.taskId + '&page=' + vue.$data.page + '&size=' + vue.$data.size;
        dsApp.get(url, function (result) {
            vue.$data.histories = result.data.result;
            vue.$data.total = result.data.total;
            dsApp.ui.initPagination('#pagination', result.data, function (page) {
                window.location.href = 'taskHistory?filter=taskId::' + vue.$data.taskId + '&page=' + page + '&size=' + vue.$data.size;
            });
        });
    }
});