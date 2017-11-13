$(function () {
    setMenuActive("running");

    var runningVue = new Vue({
        el: '#runningView',
        data: {
            tasks: [],
            size: 50,
            total: 0,
            sort: ''
        },
        mounted: function () {
            this.currentPage = 1;
            loadTasks(this);
        }
    });


    function loadTasks(vue) {
        var url = '/Task/Running';
        dsApp.post(url, { page: vue.currentPage, size: vue.size, sort: vue.sort }, function (result) {
            vue.$data.tasks = result.result.result;
            vue.$data.total = result.result.total;
            dsApp.ui.initPagination('#pagination', result.result, function (page) {
                vue.$data.page = page;
                loadTasks(vue);
            });
        });
    }
});

