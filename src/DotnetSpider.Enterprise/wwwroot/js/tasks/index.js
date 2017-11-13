$(function () {
	//setMenuActive('tasks');
	var cron;
	var interval = null;
	var tasksVUE = new Vue({
		el: '#taskContent',
		data: {
			tasks: [],
			total: 0,
			size: 10,
			keyword: '',
			page: 1,
			newTask: {
				id: 0,
				name: '',
				version: '',
				framework: 'NetCore',
				assemblyName: 'Xbjrkj.DataCollection.Apps.dll',
				taskName: '',
				isEnabled: true,
				cron: '',
				nodesCount: 1,
				extraArguments: '',
				programmer: '',
				client: '',
				executive:''
			},
			errorText: {
				name: '', version: '', taskName: '', extraArguments: '', programmer:'',client:'',executive:''
			},
			//projVersion: [],
			//templateVersion: {},
			//versions: [],
			//currentVersion: '',
			taskId: 0
		},
		mounted: function () {
			this.$data.page = 1;
			loadTasks(this);
		},
		computed: {
			buttonState:function(){
				return this.nameVdt && this.validateEmpty && this.versionVdt && this.extraArgumentsVdt && this.assemblyNameVdt && this.cronVdt && this.programmerVdt && this.executiveVdt && this.clientVdt;
			},
			nameVdt: function () {
				return this.validateEmpty(this, 'name', true);
			},
			versionVdt: function () {
				return this.validateEmpty(this, 'version', true);
			},
			taskNameVdt: function () {
				return this.validateEmpty(this, 'taskName', true);
			},
			extraArgumentsVdt: function () {
				var value = this.newTask['extraArguments'];
				if (this.validateEmpty(this, 'extraArguments', false)) {
					if (value.indexOf('-s:')>=0 || value.indexOf('-i:')>=0) {
						this.errorText["extraArguments"] = 'Arguments can not be -s or -i';
						return false;
					}
					delete this.errorText["extraArguments"];
					return true;
				}
				return false;
			},
			assemblyNameVdt: function () {
				var value = this.newTask['assemblyName'];
				if (this.validateEmpty(this, 'assemblyName', false)) {
					if (value.length > 100) {
						this.errorText["assemblyName"] = 'Less than 100 characters.';
						return false;
					}
					else if (!/^([a-zA-Z0-9]+\.){1,}(exe|dll)$/.test(value)) {
						this.errorText["assemblyName"] = 'Assembly name is not valid. eg: Xbjrkj.DataCollection.Apps.dll';
						return false;
					}
					delete this.errorText["assemblyName"];
					return true;
				}
				return false;
			},
			cronVdt: function () {
				return this.validateEmpty(this, 'cron', true);
			},
			programmerVdt: function () {
				return this.validateEmpty(this, 'programmer', true);
			},
			executiveVdt: function () {
				return this.validateEmpty(this, 'executive', true);
			},
			clientVdt: function () {
				return this.validateEmpty(this, 'client', true);
			}
		},
		methods: {
			onTriggerClick: function (event) {
				var _$modal = $('#SchedulerModal');
				cron = $("#cron1").msCron();
				cronId = null;
				_$modal.modal('show');
			},
			validateEmpty: function (vue, v, remove) {
				var value = this.newTask[v];
				if (value === '') {
					vue.errorText[v] = '';
					return null;
				}
				else if ($.trim(value) === '') {
					vue.errorText[v] = 'Value can not be empty.';
					return false;
				}
				if (remove) {
					delete vue.errorText[v];
				}
				return true;
			},
			leave: function (evt) {
				if (validator[evt.target.name] && !validator[evt.target.name](evt.target, this.newTask[evt.target.name])) {
					return;
				}
			},
			query: function () {
				tasksVUE.$data.page = 1;
				tasksVUE.$data.keyword = $('#queryKeyword').val();
				loadTasks(tasksVUE);
			},
			onKeyPress: function (evt) {
				if (evt.charCode === 13) {
					this["query"]();
				}
			},
			saveSpider: function () {

				if (Object.getOwnPropertyNames(this.errorText).length > 1) return;

				dsApp.post("/task/addTask", this.newTask, function () {
					$("#CreateNewTaskModal").modal("hide");
					loadTasks(that);
				});
			},
			run: function (task) {
				if (task.running) return;
				dsApp.post("/task/runTask", { taskId: task.id }, function () {
					swal("Operation Succeed!", "Task is prepare to run.", "success");
					setTimeout(loadStatus, 3000);
				});
			},
			deleteTask: function (task) {
				var that = this;

				swal({
					title: "Are you sure?",
					text: "Remove this task!",
					type: "warning",
					showCancelButton: true,
					confirmButtonColor: "#DD6B55",
					confirmButtonText: "Yes, do it!",
					closeOnConfirm: false
				}, function () {
					dsApp.post("/task/deleteTask", { taskId: task.id }, function () {
						swal("Operation Succeed!", "Task was removed.", "success");
						loadTasks(that);
					});
				});
			}
		}
	});

	$("#saveTrigger").bind("click", function () {
		var info = cron.getCron();
		var _$modal = $('#SchedulerModal');
		_$modal.modal('hide');
		tasksVUE.$data.newTask.cron = info.cron;
	});

	function queryString(name) {
		var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
		if (result === null || result.length < 1) {
			return "";
		}
		return result[1];
	}

	var lastQuery;

	function loadStatus() {
		var tasks = tasksVUE.$data.tasks;
		var ids = [];
		$(tasks).each(function () {
			ids.push(this.id);
		});
		dsApp.post('/Task/IsTaskRunning', { tasks: ids }, function (data) {
			$(tasks).each(function () {
				this.running = $.inArray(this.id, data.result) >= 0;
			});
		});
	}

	function loadTasks(vue) {
		var url = '/Task/GetList';
		var keywrod = vue.$data.keyword || '';
		var query = {  page: vue.$data.page, size: vue.size, keyword: keywrod };

		dsApp.post(url, query, function (result) {
			var rd = result.result.result;

			$(rd).each(function () {
				this.running = false;
			});

			vue.$data.tasks = rd;
			vue.$data.total = result.result.total;

			if (interval) {
				clearInterval(interval);
			}
			if (result.result.result.length > 0) {
				//loadStatus();
				//interval = setInterval(loadStatus, 10000);
			}

			dsApp.ui.initPagination('#pagination', result.result, function (page) {
				vue.$data.page = page;
				loadTasks(vue);
			});
		});
	}

	setTimeout(function () {
		$(".menu li").removeClass("active");
		$(".menu li a").removeClass("toggled");
		$(".menu li#tasks").addClass("active");
		$(".menu li#tasks a").addClass("toggled");
	}, 50);
});