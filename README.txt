CWC (Custom Workflow Comments) module allows you to create an item template per workflow to store additional information during the workflow process of an item. When an item is processed in the workflow a dynamic form will render the fields defined in the custom template. The values entered will be store in an Item Bucket.

UI enhancements include changes to the workflow section in the Review tab and the more links in the Workbox.

1. Install the Custom Workflow Comments (CWC) Module
2. Define a template to gather information during Workflow actions
3. Modify the template defined to inherit from templates/modules/Custom Workflow Comments/Custom Workflow Comments Base and also set the Bucketable checkbox to true
4. Modify Workflow item under sitecore/System/Workflows and set the field Custom Template to the template you created
5. Assign the Workflow to an item/template
6. Turn on the Bucket Items in the Application Options
7. Turn on the Developer Tab
7. Click on sitecore\Content\Workflow Comments, then click on Configure tab and click on Sync
8. Click on the Developer tab and click Re-Index Tree to index Workflow Comments and its dependents (at this moment, none!).
9. Once item is available in the Workbox, you can see the module work when you Submit/Approve/Reject item in workflow
10. You can check the history by clicking on the More link on the item in the Workbox or when the item is selected in the content editor, click on the Review tab and then click on History

Instructions are also available on the blog along with the video: http://nttdatasitecore.com/Blog.aspx
