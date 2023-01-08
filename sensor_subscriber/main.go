package main

import (
	"log"
	"path"
	"runtime"

	rti "github.com/rticommunity/rticonnextdds-connector-go"
)

func main(){
	_, filename, _, ok := runtime.Caller(0)
	if !ok {
		log.Panic("runtime.Caller error")
	}

	filepath := path.Join(path.Dir(filename), "./tajtekzo.xml") 
	connector, err := rti.NewConnector("SensorParticipantLibrary::SensorParticipant", filepath)
	if err != nil {
		log.Panic(err)
	}

	defer connector.Delete()
	
	input, err := connector.GetInput("sensor_data_subscriber::sensor_data_reader")
	if err != nil {
		log.Panic(err)
	}

	for {
		input.Take()
		numOfSamples, _ := input.Samples.GetLength()
		for j := 0; j < numOfSamples; j++ {
			valid, _ := input.Infos.IsValid(j)
			if valid {
				json, err := input.Samples.GetJSON(j)
				if err != nil {
					log.Println(err)
				} else {
					log.Println(string(json))
				}
			}
		}
	}
}
