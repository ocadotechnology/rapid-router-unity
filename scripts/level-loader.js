'use strict';

let unirest = require('unirest');
let fs = require('fs');

let customBlocks = {};
unirest.get('https://www.codeforlife.education/rapidrouter/api/blocks/?format=json').strictSSL(false).end((response) => {
    for (let i = 0; i < response.body.length; i++) {
        customBlocks[response.body[i].id] = response.body[i].type;
    }
    downloadLevelData();
});

function downloadLevelData() {
    let levels = [];
    unirest.get('https://www.codeforlife.education/rapidrouter/api/levels/?format=json').strictSSL(false).end((response) => {
        for (let i = 0; i < response.body.length; i++) {
            levels.push({ url: response.body[i].url, name: response.body[i].name });
        }
        levels.forEach ((level) => {
            downloadLevel(level);
            downloadBlocks(level);
        });
    });
}

function downloadLevel(level) {
    let mapUrl = level.url + 'map/?format=json';
    unirest.get(mapUrl).strictSSL(false).end((res) => {
        if (res.body) {
            let levelData = processLevel(res.body);
            writeLevelToFile(level.name, levelData);
        }
    });
}

function processLevel(level) {
	level.path = JSON.parse(level.path);
	level.origin = JSON.parse(level.origin);
	level.destinations = JSON.parse(level.destinations);
	return level;
}

function writeLevelToFile(level, data) {
    console.log("writing level: " + level + " to file");
    fs.writeFile('../Assets/Resources/Levels/' + level + '.json', JSON.stringify(data), (err) => {
        if (err) {
            console.log(err);
        } else {
            return;
        }
    });
}

function downloadBlocks(level) {
    let blockUrl = level.url + 'blocks/?format=json';
    unirest.get(blockUrl).strictSSL(false).end((res) => {
        if (res.body) {
            let levelBlockData = processBlocksToXml(res.body);
            writeBlocksToToolboxFile(level.name, levelBlockData);
        }
    });
}

function processBlocksToXml(blocksForLevel) {
    let result = '<xml id="blockly_toolbox"><category name="+">';
    blocksForLevel.forEach((block) => {
        result += `<block type="${customBlocks[block.type]}"></block>`;
    });
    result += '</category></xml>';
    return result;
}

function writeBlocksToToolboxFile(levelId, blockData) {
    console.log(`writing toolbox ${levelId} to file`);
    fs.writeFile('../Assets/Resources/Levels/' + levelId + 'Toolbox.xml', blockData, (err) => {
        if (err) {
            console.log(err);
        } else {
            return;
        }
    });
}