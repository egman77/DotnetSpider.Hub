$(function () {
    setMenuActive("nodes");
    nodesVue = new Vue({
        el: '#nodes',
        data: {
            nodes: [],
            size: 50,
            total: 0,
            sort: ''
        },
        mounted: function () {
            this.currentPage = 1;
            loadNodes(this);
        },
        methods: {
        }
    });


    function loadNodes(vue) {
        var url = '/Node/QueryNodes';
        var query = new { page: vue.currentPage, size: vue.size, sort: vue.sort };
        dsApp.post(url, query, function (result) {
            vue.$data.nodes = result.result;
            vue.$data.total = result.result.total;
            dsApp.ui.initPagination('#pagination', result.result, function (page) {
                vue.$data.page = page;
                loadNodes(vue);
            });
        });
    }
});

