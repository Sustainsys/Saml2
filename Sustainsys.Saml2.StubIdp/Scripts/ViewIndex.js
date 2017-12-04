(function () {
    "use strict";

    // Validate hidden fields too
    $.validator.setDefaults({ ignore: [] });

    var showDetails = function (show) {
        if (show) {
            $(".show-details").hide();
            $(".hide-details").show();
        } else {
            $(".hide-details").hide();
            $(".show-details").show();
        }
    };

    var setupErrorEvents = function () {
        var settings = $.data($('form')[0], 'validator').settings;
        var oldErrorPlacementFunction = settings.errorPlacement;
        settings.errorPlacement = function (error, inputElement) {
            showDetails(true);
            oldErrorPlacementFunction(error, inputElement);
        };
    };

    $(function () {
        "use strict";
        setupErrorEvents();

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
                    // Map StubIdp properties to the select2 standard text and id properties
                    valueOfElement.text = valueOfElement.DisplayName;
                    if (valueOfElement.Assertion && valueOfElement.Assertion.NameId) {
                        valueOfElement.id = valueOfElement.Assertion.NameId;
                    }
                    // Store each user in a lookup object
                    users[valueOfElement.Assertion.NameId] = valueOfElement;
                });

                data.UserList.unshift({ id: '', text: '' }); // insert empty placeholder at top

                $("#user-dropdown-placeholder").show();
                $(".show-details").show();

                // select2 item formatting template
                var formatState = function (state) {
                    if (!state.id) {
                        // Format internal select2 messages
                        return state.text;
                    }
                    if (typeof (state.cachedTemplate) === "undefined") {
                        // Format user entries, store in cache
                        state.cachedTemplate = ich.userListTemplate(state);
                    }
                    // Return formatted user
                    return state.cachedTemplate;
                };

                // Reduce the max number of shown results by setting minimumInputLength
                // This prevents input hang when searching many users
                var minimumInputLength = 0;
                if (data.UserList.length > 100) {
                    minimumInputLength = 2;
                }
                if (data.UserList.length > 1000) {
                    minimumInputLength = 3;
                }

                $("#userList").select2({
                    data: data.UserList,
                    templateResult: formatState,
                    width: "100%",
                    minimumInputLength: minimumInputLength,
                    placeholder: "Select a user"
                });

                $("#userList").focus();
                var hideDetails = (data.HideDetails || typeof (data.HideDetails) === "undefined"); // default == true
                showDetails(!hideDetails);

                // if there are validation errors displayed from server side, show the details anyway to make the errors visible
                if ($("input.input-validation-error").length > 0) {
                    showDetails(true);
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
            if (selectedUserId && users[selectedUserId]) {
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
})(); //IFFE