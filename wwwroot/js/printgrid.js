$(function () {
  print = $('#print');
  print.prepend($(document.createElement('span')).attr('class', 'k-icon k-i-print'));
  print.bind('click', function (e) {
    e.preventDefault();
    printGrid();
  });
});

function printGrid() {
  var gridElement = $('#grid'),
    printableContent = '',
    win = window.open('', '', 'width=800, height=500, resizable=1, scrollbars=1'),
    doc = win.document.open();

  var htmlStart =
    '<!DOCTYPE html>' +
    '<html>' +
    '<head>' +
    '<meta charset="utf-8" />' +
    '<title>Kendo UI Grid</title>' +
    '<link href="https://kendo.cdn.telerik.com/' + kendo.version + '/styles/kendo.common.min.css" rel="stylesheet" /> ' +
    '<style>' +
    'html { font: 11pt sans-serif; }' +
    '.k-grid { border-top-width: 0; }' +
    '.k-grid, .k-grid-content { height: auto !important; }' +
    '.k-grid-content { overflow: visible !important; }' +
    'div.k-grid table { table-layout: auto; width: 100% !important; }' +
    '.k-grid .k-grid-header th { border-top: 1px solid; }' +
    '.k-grouping-header, .k-grid-toolbar, .k-grid-pager > .k-link { display: none; }' +
    // '.k-grid-pager { display: none; }' + // optional: hide the whole pager
    '</style>' +
    '</head>' +
    '<body>';

  var htmlEnd =
    '</body>' +
    '</html>';

  var gridHeader = gridElement.children('.k-grid-header');
  if (gridHeader[0]) {
    var thead = gridHeader.find('thead').clone().addClass('k-grid-header');
    printableContent = gridElement
      .clone()
      .children('.k-grid-header').remove()
      .end()
      .children('.k-grid-content')
      .find('table')
      .first()
      .children('tbody').before(thead)
      .end()
      .end()
      .end()
      .end()[0].outerHTML;
  } else {
    printableContent = gridElement.clone()[0].outerHTML;
  }

  doc.write(htmlStart + printableContent + htmlEnd);
  doc.close();
  win.print();
  win.close();
}