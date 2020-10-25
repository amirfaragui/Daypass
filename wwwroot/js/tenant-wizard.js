var progress, tabs, currentIndex = 0;

$(function () {
  progress = $("#tenantCompleteness").data("kendoProgressBar");
  tabs = $("#tabstrip").data("kendoTabStrip");
  $(".auto-focus").focus();
});

function onSubmit(e) {
  e.preventDefault();
}

function onShow(e) {
  progress.value(currentIndex + 1);
}

function onPreviousClick(e) {
  e.preventDefault();

  tabs.select(tabs.select().prev());
}

function onNextClick(e) {
  e.preventDefault();

  tabs.select(getTabAtIndex(currentIndex + 1));
}

function onSelect(e) {

  var selectedIndex = tabIndexOfElement(e.item),

    isMovingBack = selectedIndex < currentIndex;

  if (isMovingBack || isTabValidAt(currentIndex)) {
    console.log("tab passed validation");
    currentIndex = selectedIndex;
    tabs.enable(getTabAtIndex(currentIndex), true);
  }
  else {
    e.preventDefault();
  }
}

function tabIndexOfElement(element) {
  return tabs.element.find(element).index();
}

function isTabValidAt(tabIndex) {
  var el = tabs.contentElement(tabIndex),
    val = $(el).kendoValidator().data("kendoValidator");
  return val.validate();
}

function getTabAtIndex(index) {
  return tabs.tabGroup.children().eq(index);
}

$.fn.constrainInput = function () {
  var maxRows = $(this).attr('rows');
  var maxChars = $(this).attr('cols');

  this.keypress(function (e) {
    var text = $(this).val();
    var lines = text.split('\n');
    if (e.keyCode === 13) {
      return lines.length < maxRows;
    }
    else {
      var caret = $(this).get(0).selectionStart;

      var line = 0;
      var charCount = 0;
      $.each(lines, function (i, e) {
        charCount += e.length;
        if (caret <= charCount) {
          line = i;
          return false;
        }
        //\n count for 1 char;
        charCount += 1;
      });

      var theLine = lines[line];
      return theLine.length < maxChars;
    }
  });
};
