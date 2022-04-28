# Social Post Scheduler

Post to Facebook and Twitter on a weekly schedule

## Download

[https://github.com/xCONFLiCTiONx/Social-Post-Scheduler/releases](https://github.com/xCONFLiCTiONx/Social-Post-Scheduler/releases)  

![GitHub all releases](https://img.shields.io/github/downloads/xCONFLiCTiONx/Social-Post-Scheduler/total)  [![GitHub stars](https://img.shields.io/github/stars/xCONFLiCTiONx/Social-Post-Scheduler)](https://github.com/xCONFLiCTiONx/Social-Post-Scheduler/stargazers)

## Donations

Feel free to donate to my [PayPal.Me/xCONFLiCTiONx](https://PayPal.Me/xCONFLiCTiONx) account. I work hard on these projects.

## Information

![social-post-scheduler](https://github.com/xCONFLiCTiONx/Social-Post-Scheduler/raw/main/social-post-scheduler.jpg)

![Screenshot](https://github.com/xCONFLiCTiONx/Social-Post-Scheduler/raw/main/Screenshot.jpg)

Post to Facebook, Instagram and Twitter on a weekly schedule. This is for repeat posts or for posts that you want to schedule for later. Using Facebook Creator Studio is the preferred method for posting to Facebook but for those posts that you repeat or want to post for a while then this program will save you tons of time.

## How to create the apps for Facebook, Instagram and Twitter

### Facebook API

[https://developers.facebook.com](https://developers.facebook.com)

Generate USER Token with proper permissions:

1. Make sure you are an admin of the Facebook page/s  you're posting to.
2. Create a Facebook App (should be the same user account that's associated with the page admin)
3. Head over to the [Facebook Graph API Explorer](http://developers.facebook.com/tools/explorer/)
4. On the top right, select the FB App you created from the "Application" drop down list
5. Click "Get User Access Token"
6. Make sure you add the correct permissions (see [https://github.com/xCONFLiCTiONx/Social-Post-Scheduler#facebook-permissions](Facebook Permissions) below) and then click on the `Generate Access Token` button.
7. Convert this `short-lived access token` into a `long-lived access token` by clicking on the `information ico`n just to the left of the newly created `Access Token`
  A. Click on Open in `Access Tool`
  B. At the bottom of the new page click `Extend Access Token`
  C. Click `Debug` next to the new `long-lived access token` and make sure it says `expires in about a month` and paste it into Social Post Scheduler

### Minimum Permissions

(permissions vary depending on what you're trying to accomplish)

#### Facebook Permissions

* pages_show_list
* pages_read_engagement
* pages_manage_posts

For more information on the facebook API visit [https://developers.facebook.com/docs/graph-api/](https://developers.facebook.com/docs/graph-api/)

---

#### Instagram API

To post to instagram you'll need a website with an RSS Feed. The feed must have the following:

* `<enclosure url="{image_path}" length="" type="image/jpg"/>`
* `<title>{TITLE}</title>`
* `<link>{WEBSITE_LINK}</link>`

You'll need the following namespaces:

* `xmlns:media="http://search.yahoo.com/mrss/"`
* `xmlns:ynews="http://news.yahoo.com/rss/"`

The images must be jpg and have the following aspect ratio:

* 1.91:1 aspect ratio (Example: 1200x628 and 1920x1005)
* 4:5 aspect ratio (Example: 312×390 and 800x1000)
* any aspect ratios in between 1.91:1 and 4:5

You can find your instagram Page ID here: [https://business.facebook.com/settings/instagram-account-v2s/](https://business.facebook.com/settings/instagram-account-v2s/)

*NOTE: The maximum amount of posts per day, as of this writing, is 25 which will be accounted for automatically within Social Post Scheduler. If you reach the limit then it will continue posting the next day from where it left off.*

For more information on the instagram API visit [https://developers.facebook.com/docs/instagram-api/](https://developers.facebook.com/docs/instagram-api/)

#### Instagram Permissions

* instagram_basic
* instagram_content_publish

---

#### Twitter API

[https://developer.twitter.com/en/portal/dashboard](https://developer.twitter.com/en/portal/dashboard)

* Go to Overview and click on `Create App`
* Name your app and press OK
* Copy your new keys and token to a safe place
* In the Dashboard click `App Settings` (the gear) for the app
* Click `App permissions` and choose Read and Write then Save
* Next click `Keys and tokens` near the top and generate the Access token & Secret
* If you created the `Keys and tokens` before generating the permissions then you may need to remove the app and try again
* Go to your Twitter page and check that the app is using the proper permissions: Click `More` > `Settings & Privacy` > `Security & Account Access` > `Apps and sessions`

For more information on the twitter API visit [https://developer.twitter.com/en/docs](https://developer.twitter.com/en/docs)

---

#### Setup the database

You'll need to install Microsoft Local Database to use this application. You can download it [here](https://www.microsoft.com/en-us/Download/details.aspx?id=101064).

You only need to install LocalDB
![LocalDB](/LocalDB.jpg)  

At this point everything should be working but if you've already setup Local DB before then you may need to upgrade your instance. You can check the instance using a terminal prompt: `sqllocaldb i MSSQLLocalDB`

If the outcome doesn't look like the below screenshot or match the version you've installed move to the next step.
![outcome](/outcome.jpg)

Type the following commands in a terminal:

 `sqllocaldb stop MSSQLLocalDB`

`sqllocaldb delete MSSQLLocalDB`

`sqllocaldb create MSSQLLocalDB`

Now that you've done that you should be able to run the program without issue.

#### Some things to consider while scheduling

* The ideal image size for a Facebook image post is 720px, 960px, or 2048px wide, with flexibility in the corresponding height. For best results, make sure your image is JPG format, with RGB color, and less than 15 MB. Facebook features an option to upload with high resolution, so most images can maintain their quality on the site. That said, regardless of the image type used, it's exported as jpeg and uploaded.
* Don't post to the same page at the same time. Put at least one minute between them.
* The page name field in the Facebook Connection area must be exact.
* The entries are stored in a MSSQLLocal database and the instance will shutdown to save resources. That said, if you decide to edit an entry after it's been shutdown, the initial startup can take a few seconds causing the app to freeze until the instance starts.
* Use the Text Editor tab to format the text easily along with seeing how many characters will be used. This is great for Twitter posts.
* Twitter links plus the message cannot exceed 280 characters, so use short links or images instead. Update: Twitter now shortens URL's for you that are too long. Read the documentation at [https://developer.twitter.com/en/docs/twitter-api](https://developer.twitter.com/en/docs/twitter-api) to stay updated on this information.
* You cannot post an image and a url in the same post. The url is used when you want to display the image from the website's open graph image, if they have one. If you want a link and an image in the post then put the link in the message instead.
* Keep in mind that your computer must be running and the Social Post Scheduler must be running for posts to be posted so if you reboot your computer when a post is scheduled then it will be skipped.
* To post a video, post it manually to your facebook page once and then grab the post link and then use that link in the link field to post them as a scheduled post.
* Posting too many posts at the same time could cause issues on slower networks. Spread them out over a few minutes if this happens.
* Make sure to setup backup and cleanup during a time that you won't be posting or editing the schedule.

Release Notes:

* 1.8
  * [Check for SQL connection at startup for slower computers](https://github.com/xCONFLiCTiONx/Social-Post-Scheduler/commit/e89da5cc38a750c8ad19ed38a29ad23565e8f760)
* 1.9
  * Fixed url's opening in Internet Explorer when not default
* 2.2
  * Fixed log file being deleted and never recreated
* 2.4
  * Fixed image posts missing the message #2 (comment)
* 2.5
  * Changed image type from png to jpg which resolves the upload issue
  
