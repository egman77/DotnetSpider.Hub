$(function () {
    setMenuActive("taskStatus");
    var nodesVue = new Vue({
        el: '#taskStatusesView',
        data: {
            taskStatuses: [],
            size: 50,
            total: 0,
            sort: '',
            status: 'All',
            page: 1
        },
        mounted: function () {
            loadTaskStatuses(this);
        },
        methods: {
            query: function () {
                loadTaskStatuses(this);
            }
        }
    });


    function loadTaskStatuses(vue) {
        var url = '/TaskStatus/Query';
        dsApp.post(url, { page: vue.$data.page, size: vue.size, sort: vue.sort, status: vue.status }, function (result) {
            vue.$data.taskStatuses = result.result.result;
            vue.$data.total = result.result.total;
            dsApp.ui.initPagination('#pagination', result.result, function (page) {
                vue.$data.page = page;
                loadTaskStatuses(vue);
            });
        });
    }
});

