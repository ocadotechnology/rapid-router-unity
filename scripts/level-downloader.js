'use strict';

var unirest = require('unirest');
var fs = require('fs');

var levels = [];
unirest.get('https://www.codeforlife.education/rapidrouter/api/levels/?format=json').strictSSL(false).end((response) => {
    for (var i = 0; i < response.body.length; i++) {
        levels.push({ url: response.body[i].url, name: response.body[i].name });
    }
    downloadLevels(levels);
});

function downloadLevels(levels) {
    if (levels.length > 0) {
        var level = levels.shift();
        var url = level.url + 'map/?format=json'
        unirest.get(url).strictSSL(false).end((res) => {
            if (res.body) {
                writeLevelToFile(level.name, res.body, () => {
                    downloadLevels(levels);
                });
            }
        });
    }
}

function writeLevelToFile(level, data, completion) {
    console.log("writing level: " + level + " to file");
    fs.writeFile('../Assets/Resources/Levels/' + level + '.json', JSON.stringify(data), (err) => {
        if (err) {
            console.log(err);
        } else {
            completion();
        }
    });
}