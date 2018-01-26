$(function () {
    setMenuActive("taskStatus");
    var nodesVue = new Vue({
        el: '#taskStatusesView',
        data: {
            taskStatuses: [],
            total: 0,
            sort: '',
            status: dsApp.getFilter('status') || 'All',
            keyword: decodeURIComponent(dsApp.getFilter('keyword') || ''),
            page: dsApp.queryString('page') || 1,
            size: dsApp.queryString('size') || 60,
        },
        mounted: function () {
            loadTaskStatuses(this);
        },
        methods: {
            query: function (evt) {
                if (evt) {
                    if (evt.charCode === 13) {
                        window.location.href = 'taskstatus?filter=keyword::' + nodesVue.$data.keyword + '|status::' + nodesVue.$data.status + '&page=' + 1 + '&size=' + nodesVue.$data.size;
                    }
                } else {
                    window.location.href = 'taskstatus?filter=keyword::' + nodesVue.$data.keyword + '|status::' + nodesVue.$data.status + '&page=' + 1 + '&size=' + nodesVue.$data.size;
                }
            },
            statusChanged: function (st) {
                window.location.href = 'taskstatus?filter=keyword::' + nodesVue.$data.keyword + '|status::' + st + '&page=' + 1 + '&size=' + nodesVue.$data.size;
            }
        }
    });


    function loadTaskStatuses(vue) {
        var url = 'api/v1.0/taskstatus?filter=keyword::' + vue.$data.keyword + '|status::' + vue.$data.status + '&page=' + vue.$data.page + '&size=' + vue.$data.size;
        dsApp.get(url, function (result) {
            vue.$data.taskStatuses = result.data.result;
            vue.$data.total = result.data.total;
            dsApp.ui.initPagination('#pagination', result.data, function (page) {
                window.location.href = 'taskstatus?filter=keyword::' + vue.$data.keyword + '&page=' + page + '&size=' + vue.$data.size;
            });
        });
    }
});

