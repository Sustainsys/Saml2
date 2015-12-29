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
    if ($("#AssertionModel_InResponseTo").val()) {
        $("#AssertionModel_NameId").focus().select();
    } else {
        $("#AssertionModel_AssertionConsumerServiceUrl").focus().select();
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

    $("body").on("click", ".show-details", function (e) {
        e.preventDefault();
        $(".show-details").hide();
        $(".hide-details").show("fast");
    });

    var users = {};
    var urlStart = window.location.pathname + "/";
    if (window.location.pathname[window.location.pathname.length - 1] === '/') {
        urlStart = window.location.pathname;
    }
    $.getJSON(urlStart + "Manage/CurrentConfiguration", null, function (data, textStatus, jqXHR) {
        if (data.UserList) {
            $.each(data.UserList, function (indexInArray, valueOfElement) {
                users[valueOfElement.Assertion.NameId] = valueOfElement;
            });

            $("#user-dropdown-placeholder").html(ich.userListTemplate(data));
            $("#userList").focus();
            if (data.HideDetails || typeof (data.HideDetails) === "undefined") { // default == true
                $(".hide-details").hide();
            } else {
                $(".show-details").hide();
            }

            restoreSelectedUser();
        }
    });

    $("body").on("change", "#userList", function () {
        var selectedUserId = $(this).val();
        var user = users[selectedUserId];
        $("#AssertionModel_NameId").val(selectedUserId);
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

    var cookieName = 'stubIdp.username';

    var restoreSelectedUser = function () {
        var selectedUserId = Cookies.set(cookieName);
        if (selectedUserId && $("#userList").find("option[value=" + selectedUserId + "]").length > 0) {
            $("#userList").val(selectedUserId);
            $("#userList").change();
            $("#submit").focus();
        }
    };

    $("body").on("submit", "form", function () {
        // Remember the selected user in a cookie
        var selectedUserId = $("#userList").val();
        Cookies.set(cookieName, selectedUserId, { expires: 365, path: '' }); // path: '' ensures that a separate cookie is created for each named sub-IDP
    });
});