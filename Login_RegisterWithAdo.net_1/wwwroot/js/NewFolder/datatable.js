
$(document).ready(function () {
    var table = $('#tblData').DataTable({
        "processing": true,
        "serverSide": true,
        "ajax": {
            "url": "/Register/GetAll",
            "type": "POST",
        },
        "aLengthMenu": [5, 7, 10, 15, 25],
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "email", "width": "15%" },
            { "data": "hobbies", "width": "15%" },
            { "data": "address", "width": "15%" },
            { "data": "about", "width": "15%" },
            { "data": "salary", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                      <div class="text-center">
                         <div class="btn btn-danger" >
                              <a onclick=Delete("/Register/Delete/${data}")>
                                             Remove
                              </a>
                         </div>
                      </div>
                    `;
                }
            }
        ],
    });

    $('#tblData tbody').on('click', "tr", function () {
        var modalm = $('#myModal');
        var data = table.row(this).data();
        swal({
            title: "Want To Update Data❓❔",
            text: "Click Ok❗❗",
            icon: "info",
            buttons: true
        }).then((isConfirm) => {
            if (isConfirm) {
                $.ajax({
                    type: 'GET',
                    url: '/Register/updatetable/' + data.id + '',
                    success: function (data) {
                        //console.log(data);
                        $('#txtid').val(data.data[0].id);
                        $('#txtname').val(data.data[0].name);
                        $('#txtemail').val(data.data[0].email);
                        $('#txtadd').val(data.data[0].address);
                        CKEDITOR.instances.editor.setData(data.data[0].about);
                        $('#txtsalary').val(data.data[0].salary);
                        //multiselect dropdown
                        var selectedOptions = data.data[0].hobbies.split(',');
                        for (var i = 0; i < selectedOptions.length; i++) {
                            var optionVal = selectedOptions[i];
                            $("#selectlist").find("option[value=" + optionVal + "]").prop("selected", "selected");
                            $('#selectlist').multiselect('refresh');
                        };
                    }
                });
                modalm.modal('show');
            }
            else {
                swal("Oops!", "Update Cancelled❌", "error");
            }
        });
    });
});


$(document).ready(function () {
    $("#name_error").hide();
    $("#email_error").hide();
    $("#hobbies_error").hide();
    $("#address_error").hide();
    $("#about_error").hide();
    $("#salary_error").hide();
    var name_error = false;
    var email_error = false;
    var hobbies_error = false;
    var address_error = false;
    var about_error = false;
    var salary_error = false;
    $("#txtname").keyup(function () {
        validate_name();
    });
    $("#txtemail").keyup(function () {
        validate_email();
    })
    $("#selectlist").change(function () {
        validate_hobbies();
    })
    $("#txtadd").keyup(function () {
        validate_address();
    })
    $("#txtsalary").keyup(function () {
        validate_salary();
    })
    //About
    for (var editor1 in CKEDITOR.instances) {
        CKEDITOR.instances[editor1].on('change', function validate_editor() {
            var editor = CKEDITOR.instances['editor'].getData()
            if (editor != "") {
                $("#about_error").hide()
            }
            else {
                $("#about_error").show();
                about_error = true;
            }
        })
    }
    //name
    function validate_name() {
        var name1 = $("#txtname").val();
        if (name1 != '') {
            $("#name_error").hide();
            name_error = false;
        }
        else {
            $("#name_error").show();
            name_error = true;
        }
    }
    //email
    function validate_email() {
        var pattern = /^\b[A-Z0-9._%-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b$/i
        var email1 = $("#txtemail").val();
        if (pattern.test(email1)) {
            $("#email_error").hide();
            email_error = false;
        }
        else {
            $("#email_error").show();
            email_error = true;
        }
    }
    //hobbieslist
    function validate_hobbies() {
        dropdownitem = $('#selectlist option:selected');
        if (dropdownitem.length == 0 || $(dropdownitem).val() == "") {
            $("#hobbies_error").show();
            hobbies_error = true;
        }
        else {
            $("#hobbies_error").hide();
            hobbies_error = false;
        }
    }
    //address
    function validate_address() {
        var address1 = $("#txtadd").val();
        if (address1 != '') {
            $("#address_error").hide();
            address_error = false;
        }
        else {
            $("#address_error").show();
            address_error = true;
        }
    }
    //salary
    function validate_salary() {
        var salary1 = $("#txtsalary").val();
        if (salary1 != '') {
            $("#salary_error").hide();
            salary_error = false;
        }
        else {
            $("#salary_error").show();
            salary_error = true;
        }
    }

    $("#updatebtn").click(function () {
        if (name_error == false && email_error == false && hobbies_error == false && about_error == false && address_error == false && salary_error == false) {
            $.ajax({
                type: 'POST',
                url: '/Register/updatetable',
                data: {
                    Id: $('#txtid').val(),
                    Name: $('#txtname').val(),
                    Email: $('#txtemail').val(),
                    Hobbies: $('#selectlist').val().join(', '),
                    Address: $('#txtadd').val(),
                    About: CKEDITOR.instances.editor.getData(),
                    Salary: $('#txtsalary').val(),

                },
                success: function () {
                    // console.log(data);
                    swal("Good Job💯🤩🤩", "Data Updated Successfully!", "success").then(function () {
                        location.reload();
                    })
                },
                error: function () {
                    swal("Error!", "Please try again", "error");
                    $('#tblData').DataTable().ajax.reload();
                }
            })
            $('#myModal').modal('hide');
        }
        else {
            $('myModal').modal('show');
        }

    });

});
function Delete(url) {
    /*swal({
        title: "Want to delete data?",
        text: "Delete Information!!",
        buttons: true,
        icon: "warning",
        dangerModel: true

    }).then((willdelete) => {
        if (willdelete) {*/
    $.ajax({
        url: url,
        type: "Delete",
        success: function () {
            swal({
                title: 'data  deleted successfully💯💯💯',
                closeOnConfirm: false,
            }).then(function () {
                window.location.href = "/Register/Index";
            })
        }
    })

        
   // })
}



