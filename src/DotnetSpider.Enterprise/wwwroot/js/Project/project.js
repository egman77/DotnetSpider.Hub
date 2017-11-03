$(function () {
	var lastSelect = null;
	var vue = new Vue({
		el: '#container',
		data: {
			projects: [],
			current: {
				name: "",
				client: "",
				executive: "",
				note: "",
				isEnabled: true
			},
			onEdit: false,
			status: {
				buildId: 0,
				logs: [],
				status: "Waitting",
				filePath: ""
			}
		},
		mounted: function () {
			loadProjects();
		},
		methods: {
			clone: function (obj1, obj2) {
				obj1.id = obj2.id;
				obj1.name = obj2.name;
				obj1.client = obj2.client;
				obj1.executive = obj2.executive;
				obj1.isEnabled = obj2.isEnabled;
				obj1.note = obj2.note;
			},
			addProject: function () {
				lastSelect = null;
				this.onEdit = false;
				$("#NewRoleModal").modal("show");
				this.clone(this.current, { name: "", client: "", executive: "", note: "", isEnabled: true });
			},
			editProject: function (proj) {
				lastSelect = proj;
				this.onEdit = true;
				$("#NewRoleModal").modal("show");
				this.clone(this.current, proj);
			},
			saveProject: function () {
				var that = this;
				if (lastSelect) {
					var item = JSON.parse(JSON.stringify(lastSelect));
					this.clone(item, this.current);

					dsApp.post("/project/modifyProject", item, function () {
						that.clone(lastSelect, item);
						$("#NewRoleModal").modal("hide");
					});
				}
				else {
					dsApp.post("/project/addProject", this.current, function (msg) {
						that.projects.splice(0, 0, msg.result);
						$("#NewRoleModal").modal("hide");
					});
				}
			},
			removeProject: function (proj) {
				var that = this;
				swal({
					title: "Are you sure?",
					text: "You will not be able to recover this project!",
					type: "warning",
					showCancelButton: true,
					confirmButtonColor: "#DD6B55",
					confirmButtonText: "Yes, do it!",
					closeOnConfirm: false
				}, function () {
					dsApp.post("/project/RemoveProject", { projectId: proj.id }, function () {
						swal.close();
						var idx = that.projects.indexOf(proj);
						that.projects.splice(idx, 1);
					});
				});
			},
			disableProject: function (proj) {
				swal({
					title: "Are you sure?",
					text: "To disable this project!",
					type: "warning",
					showCancelButton: true,
					confirmButtonColor: "#DD6B55",
					confirmButtonText: "Yes, do it!",
					closeOnConfirm: false
				}, function () {
					dsApp.post("/project/enableOrDisableProject", { projectId: proj.id, enabled: false }, function () {
						swal("Disabled!", "Project disabled.", "success");
						loadProjects();
					});
				});
			},
			enableProject: function (proj) {
				dsApp.post("/project/enableOrDisableProject", { projectId: proj.id, enabled: true }, function () {
					swal("Enabled!", "Project enabled.", "success");
					loadProjects();
				});
			}
		}
	});

	function viewStatus(buildId) {
		$("#LogsModal").modal("show");
		vue.$data.status.buildId = buildId;
		refreshStatus();
	}

	function refreshStatus() {
		dsApp.post("/project/GetStatusAndLogs", { buildId: vue.$data.status.buildId }, function (msg) {
			vue.$data.status = msg.result;
			if (msg.result.status == 'Finished' || msg.result.status == 'Exception') return;
			setTimeout(refreshStatus, 5000);
		});
	}

	function loadProjects() {
		dsApp.get("/project/list", function (msg) {
			vue.$data.projects = msg.result || [];
		});
	}

	setTimeout(function () {
		$(".menu li").removeClass("active");
		$(".menu li a").removeClass("toggled");
		$(".menu li#projects").addClass("active");
		$(".menu li#projects a").addClass("toggled");
	}, 50)
});
