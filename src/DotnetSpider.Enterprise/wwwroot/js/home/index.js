$(function () {
    $('.count-to').countTo();

    //Sales count to
    $('.sales-count-to').countTo({
        formatter: function (value, options) {
            return '$' + value.toFixed(2).replace(/(\d)(?=(\d\d\d)+(?!\d))/g, ' ').replace('.', ',');
        }
    });

    setMenuActive("home");
    var nodesVue = new Vue({
        el: '#homeView',
        data: {
            dashboard: {
                nodes: []
            }
        },
        mounted: function () {
            loadDashboard(this);
        },
        methods: {
            signOut: function () {
                var that = this;
                that.logsPaging.total = 0;
                that.logsPaging.page = 1;
                lastQuery = null;
                loadLogs();
            }
        }
    });


    function loadDashboard(vue) {
        var url = '/Home/Dashboard';
        dsApp.post(url, null, function (result) {
            vue.$data.dashboard = result.result;
        });
    }
});