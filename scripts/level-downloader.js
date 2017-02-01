'use strict';

let unirest = require('unirest');
let fs = require('fs');

let levels = [];
unirest.get('https://www.codeforlife.education/rapidrouter/api/levels/?format=json').strictSSL(false).end((response) => {
    for (let i = 0; i < response.body.length; i++) {
        levels.push({ url: response.body[i].url, name: response.body[i].name });
    }
    downloadLevels(levels);
});

function downloadLevels(levels) {
    if (levels.length > 0) {
        let level = levels.shift();
        let url = level.url + 'map/?format=json'
        unirest.get(url).strictSSL(false).end((res) => {
            if (res.body) {
            	let levelData = processLevel(res.body);
                writeLevelToFile(level.name, levelData, () => {
                    downloadLevels(levels);
                });
            }
        });
    }
}

function processLevel(level) {
	level.path = JSON.parse(level.path);
	level.origin = JSON.parse(level.origin);
	level.destinations = JSON.parse(level.destinations);
	return level;
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