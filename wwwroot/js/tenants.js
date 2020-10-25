"use strict";

var hubConnectionId = "";
var create = null;
var newTab = null;

$.fn.serializeObject = function () {
  var o = {};
  this.serializeArray().map(function (x) { o[x.name] = x.value; });
  return o;
};

var connection = new signalR.HubConnectionBuilder()
  .withUrl("/tenantHub")
  .configureLogging(signalR.LogLevel.Trace)
  .build();

connection.on("connectionEstablished", function (connectionId) {
  hubConnectionId = connectionId;
  console.log('my hub connection is ' + connectionId);
});

connection.on("ViewUpdateSuggested", function () {
  var grid = $('#grid').data('kendoGrid');
  grid.dataSource.read();

  if (newTab !== null) {
    newTab.close();
    newTab = null;
  }
});


function onRowBound(e) {
  $(".k-grid-Edit").prepend($(document.createElement('span')).attr('class', 'k-icon k-i-edit'));
}


function onEdit(e) {
  e.preventDefault();
  var dataItem = this.dataItem($(e.currentTarget).closest("tr"));

  var url = '/Admin/tenants/edit/' + dataItem.Id + '?clientId=' + hubConnectionId
  //newTab = window.open(url, '_blank');
  window.location.href = url;
}

$(function () {
  create = $('#create');
  create.prepend($(document.createElement('span')).attr('class', 'k-icon k-i-plus'));
  create.attr('disabled', 'disabled');

  connection.start().then(function () {
    console.log('connection established');
    create.removeAttr('disabled');

    create.bind('click', function (e) {
      e.preventDefault();

      var url = '/Admin/tenants/create?clientId=' + hubConnectionId;
      //newTab = window.open(url, '_blank');
      window.location.href = url;
    });
  }).catch(function (err) {
    return console.error(err.toString());
  });

});


function showDialog(style, title, message) {
  $('#dialog').removeClass('panel-danger');
  $('#dialog').removeClass('panel-success');
  $('#dialog').removeClass('panel-warning');
  $('#dialog').addClass(style);
  $('#header').empty();
  $('#header').append(title);

  $('#message').empty();
  $('#message').append(message);

  $('#confirmation').show();
  $('#progress').hide();

  $('#modalPopup').show();
}
