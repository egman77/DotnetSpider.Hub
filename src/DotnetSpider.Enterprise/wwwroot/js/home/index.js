$(function () {
    $('.count-to').countTo();

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
        }
    });

    function loadDashboard(vue) {
        var url = '/api/v1.0/report?filter=type::homedashboad';
        dsApp.get(url, function (result) {
            vue.$data.dashboard = result.data;
        });
    }
});