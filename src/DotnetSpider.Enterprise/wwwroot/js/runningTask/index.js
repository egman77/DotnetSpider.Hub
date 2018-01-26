$(function () {
    setMenuActive("running");

    var runningVue = new Vue({
        el: '#runningView',
        data: {
            tasks: [],
            total: 0,
            page: dsApp.queryString('page') || 1,
            size: dsApp.queryString('size') || 60,
        },
        mounted: function () {
            loadTasks(this);
        }
    });


    function loadTasks(vue) {
        var url = 'api/v1.0/task?filter=isRunning::true&page=' + vue.$data.page + '&size=' + vue.$data.size;
        dsApp.get(url, function (result) {
            vue.$data.tasks = result.data.result;
            vue.$data.total = result.data.total;
            dsApp.ui.initPagination('#pagination', result.data, function (page) {
                window.location.href = 'task?filter=isRunning::true&page=' + page + '&size=' + vue.$data.size;
            });
        });
    }
});

