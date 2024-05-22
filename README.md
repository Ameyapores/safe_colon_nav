# DVC
This is the codebase for Constrained Reinforcement Learning and Formal Verification for Safe Colonoscopy Navigation. 
This repository contains the Unity scene used to implement autonomous control for colonoscopy. 

Find the [paper](https://ieeexplore.ieee.org/abstract/document/10341789)

<img src="images/video.gif" width="300"> 

## Prerequisites/Packages
- mlagents 0.16.1

## How to run

To run this demo you need to have Unity-64bit (has been tested on version 2019.4.13f1) installed on your system.
Afterwards, you have to:
1. Clone or download this repo.
2. Start Unity.
3. Select 'Open Project' and select the root folder that you have just cloned.
4. Press 'Play' button to run a sample trajectory.

## How to train
[MlAgents toolkit](https://github.com/Unity-Technologies/ml-agents) can be used provide Unity technologies. We present the second way here:

Follow the instruction on this page to start the training.: [Training mlagents](https://github.com/Unity-Technologies/ml-agents/blob/release_2_docs/docs/Training-ML-Agents.md). Following are the steps:
```
git clone https://github.com/Unity-Technologies/ml-agents/tree/release_2
cd ml-agents-release_2
mlagents-learn config/trainer_config.yaml --run-id training
```
Press 'Play' button from the Unity editor to start training.
The training can be visualised using tensorboard
```
cd ml-agents-release_2
tensorboard --logdir summaries --port 6006
```

## Network architecture
<img src="images/conv.png"> 
