$(function () {
    setMenuActive("nodes");
    nodeIndexPageVue = new Vue({
        el: '#nodeList',
        data: {
            nodes: []
        },
        mounted: function () {
            loadNodes(this);
        },
        methods: {
        }
    });


    function loadNodes(vue) {
        var url = '/Node/GetCurrentNodeInfo';
        dsApp.post(url, null, function (result) {
            vue.$data.nodes = result.result;
            setTimeout(() => {
                $('#nodeList').DataTable({
                    destroy: true,
                    responsive: true,
                    bFilter: true,
                    bLengthChange: true
                });
            });
        });
    }

    function time(f, vue, t) {
        return function walk() {
            setTimeout(function () {
                f(vue);
                walk();
            }, t);
        };
    }

    time(loadNodes, nodeIndexPageVue, 5000)();
});

