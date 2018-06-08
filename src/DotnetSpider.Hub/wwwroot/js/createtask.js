$(function () {
    var view = new Vue({
        el: '#view',
        data: {
            task: {
                id: 0,
                name: '',
                version: '',
                os: 'all',
                applicationName: 'dotnet',
                isEnabled: true,
                cron: '',
                nodeCount: 1,
                arguments: '',
                owners: '',
                developers: '',
                analysts: '',
                tags: '',
                nodeType: 0,
                isSingle: true
            },
            errorText: {
                name: '', applicationName: '', version: '', arguments: '', nodeCount: ''
            }
        }, mounted: function () {
            $('select').formSelect();
        },
        computed: {
            validate: function () {
                var nv = this.notEmpty('name')
                var vv = this.notEmpty('version');
                var av1 = this.notContains('arguments', '--tid');
                var av2 = this.notContains('arguments', '--identity');
                var av3 = this.notContains('arguments', '-i');
                var anv = this.notEmpty('applicationName');
                var cv = this.notEmpty('cron');
                var ncv = this.largeThanZero('nodeCount');
                return nv && vv && av1 && av2 && av3 && anv && cv && ncv;
            }
        },
        methods: {
            largeThanZero: function (name) {
                var value = this.$data.task[name];
                if (value >= 1) {
                    delete this.errorText[name];
                    return true;
                }
                return false;
            },
            notContains: function (name, contains) {
                var value = this.$data.task[name];
                if (value && (value.indexOf(contains) >= 0)) {
                    errorText[name] = name + ' should not contains ' + contains;
                    return false;
                }
                delete this.errorText[name];
                return true;
            },
            notEmpty: function (name) {
                var value = this.$data.task[name];
                if (value === '') {
                    this.errorText[name] = '';
                    return null;
                }
                else if ($.trim(value) === '') {
                    this.$data.errorText[name] = name + ' can not be empty.';
                    return false;
                }
                this.errorText[name] = '';
                return true;
            },
            create: function () {
                if (Object.getOwnPropertyNames(this.errorText).length > 1) return;
                var task = this.$data.task;
                hub.post("/api/v1.0/task", task, function () {
                    window.location.href = '/task';
                });
            }
        }
    });
});