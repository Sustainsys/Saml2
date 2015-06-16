$(function () {
    "use strict";

    var resetUnobtrusive = function (form) {
        if (form && form.length) {
            form.off("submit"); // remove previous click event

            form.removeData('validator');
            form.removeData('unobtrusiveValidation');
            $.validator.unobtrusive.parse(form);

            var formData = form.data("validator");
            if (formData) {
                formData.settings.ignore = ":hidden:not(.AlwaysValidate), .SkipValidation";
            }
        }
    };

    // Focus first relevant field to use
    if ($("#InResponseTo").val()) {
        $("#NameId").focus().select();
    } else {
        $("#AssertionConsumerServiceUrl").focus().select();
    }

    var attributeCount = 0;

    $("#add-attribute").click(function (e) {

        e.preventDefault();
        var rowInfo = {
            rowIndex: attributeCount
        };
        var newRow = ich.attributeRowTemplate(rowInfo);
        $("#attributes-placeholder").append(newRow).show();
        resetUnobtrusive($("form"));
        attributeCount++;
    });

    $("body").on("click", ".remove-attribute", function (e) {
        e.preventDefault();
        $(e.target).closest(".attribute-row").remove();
    });

    var users = {};
    $.getJSON("Manage/CurrentConfiguration", null, function (data, textStatus, jqXHR) {
        if (data.UserList) {
            $.each(data.UserList, function (indexInArray, valueOfElement) {
                users[valueOfElement.Assertion.NameId] = valueOfElement;
            });

            $("#user-dropdown-placeholder").html(ich.userListTemplate(data));
            $("#userList").focus();
        }
    });

    $("body").on("change", "#userList", function () {
        var selectedUserId = $(this).val();
        var user = users[selectedUserId];
        $("#NameId").val(selectedUserId);
        if (user && user.Description) {
            $("#userDescription").text(user.Description);
        }
        else {
            $("#userDescription").text('');
        }

        $(".attribute-row").remove();

        attributeCount = 0;
        if (user && user.Assertion && user.Assertion.AttributeStatements) {
            $.each(user.Assertion.AttributeStatements, function (idx, element) {

                var rowInfo = {
                    type: element.Type,
                    value: element.Value,
                    rowIndex: attributeCount
                };
                var newRow = ich.attributeRowTemplate(rowInfo);
                $("#attributes-placeholder").append(newRow).show();
                attributeCount++;
            });
            resetUnobtrusive($("form"));
        }
    });
});