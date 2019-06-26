$(function () {
	$("#myTable").DataTable({
		ajax: {
			url: Url_GetData,
			type: "POST",
			contentType: "application/json",
		},
		columns: [
			{ data: "Id" },
			{ data: "Forename" },
			{ data: "Surname" },
			{ data: "LandlordName" },
			//etc...
		],
		lengthMenu: [[25, 50, -1], [25, 50, "All"]],
		processing: true,
		serverSide: true,
	});
});