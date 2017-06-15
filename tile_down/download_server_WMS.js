/**
 * Created by lenovo on 2017/3/10.
 */

//var m_config = require('./config_tbo.json');

var m_config = require('./config.json');
var path = require('path');
var fs = require('fs');
var http = require('http');
var request = require('request');
var async = require('async');
var DownLoadList = [];
var m_DownloadNum = 0;
var m_downloadNow = 0;
var FilePath = "";
//循环逻辑
getdownload_Init();

function getdownload_Init() {
    var m_downLoadList = m_config.downloadList;
    var m_download1 = m_downLoadList[0];
    //下载路径
    var m_URL = "http://www.scgis.net.cn/imap/iMapServer/defaultRest/services/newtianditudom_scann/WMS?LAYERS=0&TRANSPARENT=TRUE&ISBASELAYER=false&SERVICE=WMS&VERSION=1.1.1&REQUEST=GetMap&STYLES=&FORMAT=image%2Fpng&SRS=EPSG%3A4326&BBOX=lon_0,lat_0,lon_1,lat_1&WIDTH=256&HEIGHT=256";
    FilePath = m_download1.FilePath;
    //下载文件名称
    //文件夹名称
    var TileNum = m_download1.threadNum;

    var TotalCount = 0;
    var all_DownLoadList = [];
    for (var z = 0; z <= TileNum; z++) {
        var xMax = Math.pow(2, z + 1) - 1;
        var yMax = Math.pow(2, z) - 1;
        for (let x = 0; x <= xMax; x++) {
            for (let y = 0; y <= yMax; y++) {
                TotalCount++;
                var m_DownLoad = [];
                m_DownLoad.x = x;
                m_DownLoad.y = y;
                m_DownLoad.z = z;

                var levelsize = 0;
                switch (z) {
                    case 0:
                    {
                        levelsize = 360;
                        break;
                    }
                    case 1:
                    {
                        levelsize = 360 / 2;
                        break;
                    }
                    case 2:
                    {
                        levelsize = 360 / 4;
                        break;
                    }
                    case 3:
                    {
                        levelsize = 360 / 8;
                        break;
                    }
                    case 4:
                    {
                        levelsize = 360 / 16;
                        break;
                    }
                    case 5:
                    {
                        levelsize = 360 / 32;
                        break;
                    }
                    case 6:
                    {
                        levelsize = 360 / 64;
                        break;
                    }
                    case 7:
                    {
                        levelsize = 360 / 128;
                        break;
                    }
                    case 8:
                    {
                        levelsize = 360 / 256;
                        break;
                    }
                    default:
                        break;
                }
                levelsize = levelsize / 2;
                var xleft = -180 + x * levelsize;

                var xright = -180 + (x + 1) * levelsize;

                var ytop = -90 + y * levelsize;

                var ybottom = -90 + (y + 1) * levelsize;

                m_DownLoad.Url = m_URL.replace("lon_0", xleft).replace("lon_1", xright).replace("lat_0", ytop).replace("lat_1", ybottom);
                /*   fs.appendFile("log.txt", m_DownLoad.Url);
                 fs.appendFile("log.txt", "\n");*/
                var filename = path.resolve(__dirname, 'download/' + FilePath + '/' + (m_DownLoad.z ) + "/" +
                    m_DownLoad.x + "/" + m_DownLoad.y + ".png");
                if (!fs.existsSync(filename)) {
                    //console.log(xleft + "," + xright + "," + ytop + "," + ybottom);
                    all_DownLoadList.push(m_DownLoad);
                }
            }
        }
    }

    m_DownloadNum = all_DownLoadList.length;
    DownLoadList = all_DownLoadList;
    console.log("待下载总数：" + m_DownloadNum);
    console.log("待下载分组：" + DownLoadList.length);
    download_async();
}

//异步下载控制


function download_async() {
    var q = async.queue(function (task, callback) {
        download_file_httpget(task, callback);
        //  callback();
    }, 3);
    q.push(DownLoadList, function (err) {
        //console.log('finished processing item');
    });
    q.drain = function () {
        console.log('finished processing ALL');
    };
}

function download_200(task_200, callback1) {
    console.log('download_200:' + task_200.length);
    var q = async.queue(function (task, callback) {
        download_file_httpget(task, callback);
        //  callback();
    }, 3);
    q.push(task_200, function (err) {
        var startTime = new Date().getTime();
        while (new Date().getTime() < startTime + 200);
        //console.log('finished processing item');
    });
    q.drain = function () {
        // console.log("完成200!");
        var startTime = new Date().getTime();
        while (new Date().getTime() < startTime + 30000);
        callback1();
    };
}

function download_file_httpget(downLoad_item, callback) {

    // console.log(downLoad_item);
    var m_url = downLoad_item.Url;
    var m_ImgName = downLoad_item.y + ".png";
    //  console.log(__dirname);
    var dirpath = path.resolve(__dirname, 'download/' + FilePath + '/' + (downLoad_item.z ) + "/" + downLoad_item.x + "/");
    // console.log(dirpath);
    //循环查找文件
    if (!fs.existsSync(dirpath)) {
        var pathtmp;
        dirpath.split(path.sep).forEach(function (dirname) {
            if (pathtmp) {
                pathtmp = path.join(pathtmp, dirname);
            }
            else {
                pathtmp = dirname;
            }
            var m_type = '0755';
            //  console.log(pathtmp);
            if (!fs.existsSync(pathtmp)) {
                if (!fs.mkdirSync(pathtmp, m_type)) {
                    return false;
                }
            }
        });
    }
    var filename = dirpath + "/" + m_ImgName;
    //User-Agent: Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; 360SE)
    //Mozilla/5.0 (Windows NT 6.1; WOW64; rv:47.0) Gecko/20100101 Firefox/47.0
    //User-Agent:
    // Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; Trident/4.0; SE 2.X MetaSr 1.0; SE 2.X MetaSr 1.0; .NET CLR 2.0.50727; SE 2.X MetaSr 1.0)
    var headers = {
        'Connection': 'keep-alive',
        'Cache-Control': 'max-age=0',
        'Upgrade-Insecure-Requests': '1',
        'User-Agent': 'User-Agent: Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; 360SE)',
        'Accept': 'text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8',
        'Accept-Encoding': 'gzip, deflate, sdch',
        'Accept-Language': 'zh-CN,zh;q=0.8',
        'Cookie': '_gscu_391188833=865333412g12s384',
        'If-None-Match': '"570261a4-d5"'
    };

    var options = {
        url: m_url,
        headers: {
            'User-Agent': 'User-Agent: Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; 360SE)',
            'Accept': 'text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8',
            'Accept-Encoding': 'gzip, deflate, sdch',
            'Accept-Language': 'zh-CN,zh;q=0.8'
        }
    };
    //获取文件
    if (!fs.existsSync(filename)) {
        try {
            console.log("开始下载文件：" + m_url);
            var stream = request(options).pipe(
                fs.createWriteStream(filename)
            );
            stream.on('finish', function () {
                console.log("下载文件完成：" + filename);
                m_downloadNow++;
                console.log(m_downloadNow + "/" + m_DownloadNum + "----" + (m_downloadNow * 100 / m_DownloadNum).toFixed(2) + "%");
                callback();
            });
        }
        catch (err) {
        }
    }
    else {
        callback();
    }

    return true;

}

