SiteCron module provides an advanced way to run Cron based scheduling jobs using Sitecore and Quartz Scheduler. It uses the CronTrigger functionality to help you schedule simple to complex jobs.

Add your scheduled jobs at /sitecore/system/Modules/Sitecron

You can get more information about the Cron triggers and examples from Quartz Scheduler website at: http://www.quartz-scheduler.net/documentation/quartz-2.x/tutorial/crontriggers.html

Sitecron comes with a sample script called SampleLogJob which logs an info log entry based on the schedule you set. Make sure any implementations of schedule jobs inherit Quartz.IJob.

Thank you for using Sitecron.

Instructions are also available on the blog along with the video: http://www.akshaysura.com
