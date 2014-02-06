var reminderApp = function () {
    (function ($) {
        var createMappingFunctions = function(displayFieldSelector, editFieldSelector, projectDisplayText) {
            var display = $(displayFieldSelector);
            var edit = $(editFieldSelector);

            return {
                shownHandler: function(e) {
                    display.hide();
                    edit.show();
                },
                hiddenHandler: function(e) {
                    edit.hide();
                    display.show();
                },
                changeHandler: function(e) {
                    display.text(projectDisplayText(edit));
                }
            };
        };

        var valueEditingFunctions = createMappingFunctions(
            '#date-span-selection-value',
            '#date-span-selection-value-input',
            function(input) {
                return input.val();
            }
        );

        var unitEditingFunctions = createMappingFunctions(
            '#date-span-selection-unit',
            '#date-span-selection-unit-select',
            function(select) {
                return select.find('option:selected').text();
            }
        );

        $('#date-span-selection-value')
            .click(valueEditingFunctions.shownHandler);

        $('#date-span-selection-value-input')
            .blur(valueEditingFunctions.hiddenHandler)
            .change(valueEditingFunctions.changeHandler);

        $('#date-span-selection-unit')
            .click(unitEditingFunctions.shownHandler);

        $('#date-span-selection-unit-select')
            .blur(unitEditingFunctions.hiddenHandler)
            .change(unitEditingFunctions.changeHandler);

    })(jQuery);
};