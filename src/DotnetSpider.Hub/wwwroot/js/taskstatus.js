$(function () {
    var view = new Vue({
        el: '#view',
        data: {
            taskStatuses: [],
            total: 0,
            sort: '',
            status: hub.queryString('status') || 'All',
            keyword: hub.queryString('keyword') || '',
            page: hub.queryString('page') || 1,
            size: hub.queryString('size') || 60,
        },
        mounted: function () {
            var that = this;
            var url = 'api/v1.0/taskstatus?keyword=' + that.$data.keyword + '&status=' + that.$data.status + '&page=' + that.$data.page + '&size=' + that.$data.size;
            hub.get(url, function (result) {
                that.$data.taskStatuses = result.data.result;
                that.$data.total = result.data.total;
                that.$data.page = result.data.page;
                that.$data.size = result.data.size;

                hub.ui.initPagination('#pagination', result.data, function (page) {
                    window.location.href = 'taskstatus?keyword=' + that.$data.keyword + '&status=' + that.$data.status + '&page=' + page + '&size=' + that.$data.size;
                });
            });

            $('select').formSelect();
        },
        methods: {
            query: function (evt) {
                var redirect = (evt && evt.charCode === 13) || !evt;
                if (redirect) {
                    var url = 'taskstatus?keyword=' + this.$data.keyword + '&status=' + this.$data.status + '&page=' + 1 + '&size=' + this.$data.size;
                    window.location.href = url;
                }
            }
        }
    });
});

