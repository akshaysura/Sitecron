<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SiteCron.aspx.cs" Inherits="Sitecron.Core.Admin.SiteCronAdminPage" %>

<!DOCTYPE html>
<%--EXPERIMENTAL at this moment Friends!--%>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SiteCron Job Viewer</title>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" integrity="sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u" crossorigin="anonymous">
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap-theme.min.css" integrity="sha384-rHyoN1iRsVXV4nD0JutlnGaslCJuC7uwjduW9SVrLvRYooPp2bWYgmgJQIXwl/Sp" crossorigin="anonymous">
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js" integrity="sha384-Tc5IQib027qvyjSMfHjOMaLkfuWVxZxUPnCJA7l2mCWNIpG9mGCD8wGNIcPD7Txa" crossorigin="anonymous"></script>
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <style>
        .row {
            margin-bottom: 10px;
        }

            .row .row {
                margin-top: 10px;
                margin-bottom: 0;
            }

        .block {
            background-color: rgba(245,245,245,0.70);
        }

        .col-lg-1, .col-lg-10, .col-lg-11, .col-lg-12, .col-lg-2, .col-lg-3, .col-lg-4, .col-lg-5, .col-lg-6, .col-lg-7, .col-lg-8, .col-lg-9, .col-md-1, .col-md-10, .col-md-11, .col-md-12, .col-md-2, .col-md-3, .col-md-4, .col-md-5, .col-md-6, .col-md-7, .col-md-8, .col-md-9, .col-sm-1, .col-sm-10, .col-sm-11, .col-sm-12, .col-sm-2, .col-sm-3, .col-sm-4, .col-sm-5, .col-sm-6, .col-sm-7, .col-sm-8, .col-sm-9, .col-xs-1, .col-xs-10, .col-xs-11, .col-xs-12, .col-xs-2, .col-xs-3, .col-xs-4, .col-xs-5, .col-xs-6, .col-xs-7, .col-xs-8, .col-xs-9 {
            padding-right: 7px;
            padding-left: 7px;
        }

        @media (min-width: 1200px) {
            .dl-horizontal dt {
                float: left;
                width: 295px;
                overflow: hidden;
                clear: left;
                text-align: left;
                text-overflow: ellipsis;
                white-space: nowrap;
            }
            .dl-horizontal dd {
                margin-left: 295px;
            }
        }
    </style>
</head>
<body>
    <div class="jumbotron">
        <div class="container">
            <div class="row">
                <div class="col-md-12">
                    <a href="/sitecore/admin/">Administration Tools</a> - <h1>SiteCron Jobs Viewer</h1>
                </div>
            </div>
        </div>
    </div>

       
    <div class="container">
        <div class="row">
            <div class="col-md-12">
                <div class="col-md-12">
					Refresh each <a href='sitecron.aspx' class='refresh-link refresh-selected'>No Refresh</a>, <a href='sitecron.aspx?refresh=1' class='refresh-link '>1 sec</a>, <a href='sitecron.aspx?refresh=2' class='refresh-link '>2 sec</a>, <a href='sitecron.aspx?refresh=5' class='refresh-link '>5 sec</a>, <a href='sitecron.aspx?refresh=10' class='refresh-link '>10 sec</a>, <a href='sitecron.aspx?refresh=20' class='refresh-link '>20 sec</a>, <a href='sitecron.aspx?refresh=30' class='refresh-link '>30 sec</a>, <a href='sitecron.aspx?refresh=60' class='refresh-link '>60 sec</a><br /><br />
					<h3>Scheduled Jobs</h3>
                    <div class="table-responsive">
                        <table class="table">
                            <thead>
                                <tr>
                                    <th>Group</th>
                                    <th>JobName</th>
                                    <th>Type</th>
                                    <th>State</th>
                                    <th>NextFireTimeUtc</th>
                                </tr>
                            </thead>
                            <tbody>
                                <%foreach (var job in this.QuartzJobs)
                                    { %>
                                <tr>
                                    <td><%=job.Group %></td>
                                    <td><%=job.JobName %></td>
                                    <td><%=job.Type %></td>
                                    <td><%=job.State %></td>
                                    <td><%=job.NextFireTimeUtc %></td>
                                </tr>
                                <%} %>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        function getQueryString() {
            var result = {}, queryString = location.search.substring(1), re = /([^&=]+)=([^&]*)/g, m;
            while (m = re.exec(queryString)) {
                result[decodeURIComponent(m[1])] = decodeURIComponent(m[2]);
            }

            return result;
        }

        var str = getQueryString()["refresh"];
        if (str != undefined) {
            c = parseInt(str) * 1000;
            setTimeout("document.location.href = document.location.href;", c);
        }
    </script>
</body>
</html>
