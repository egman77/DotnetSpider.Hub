$(function () {
    var view = new Vue({
        el: '#view',
        data: {
            tasks: [],
            total: 0,
            page: hub.queryString('page') || 1,
            size: hub.queryString('size') || 60,
        },
        mounted: function () {
            loadView(this);
        }
    });

    function loadView(vue) {
        var url = 'api/v1.0/task?isrunning=true&page=' + vue.$data.page + '&size=' + vue.$data.size;
        hub.get(url, function (result) {
            vue.$data.tasks = result.data.result;
            vue.$data.total = result.data.total;
            vue.$data.page = result.data.page;
            vue.$data.size = result.data.size;
            hub.ui.initPagination('#pagination', result.data, function (page) {
                window.location.href = 'task?isrunning=truepage=' + page + '&size=' + vue.$data.size;
            });
        });
    }
});

