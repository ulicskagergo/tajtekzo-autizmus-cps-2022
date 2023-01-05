from datetime import datetime, timedelta
from time import sleep, time
import random

import rticonnextdds_connector as rti

connector = rti.Connector(url='./tajtekzo.xml', config_name='SensorParticipantLibrary::SensorParticipant')


while True:
    output = connector.get_output('sensor_data_publisher::sensor_data_writer')
    output.instance['time'] = time()
    output.instance['host'] = 'simulator'
    output.instance['sensor_name'] = 'light_sensor'
    rand = random.uniform(0, 1000)
    output.instance['value'] = rand
    print(rand)
    output.write()
    sleep(5)