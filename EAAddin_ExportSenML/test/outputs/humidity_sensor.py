from datetime import datetime, timedelta
from time import sleep, time
import random
import rticonnextdds_connector as rti

connector = rti.Connector(url='./humidity_sensor.xml', config_name='SensorParticipantLibrary::SensorParticipant')

while True:
    output = connector.get_output('sensor_data_publisher::sensor_data_writer')
    output.instance['time'] = time()
    output.instance['host'] = 'simulator'
    output.instance['sensor_name'] = 'humidity_sensor'
    output.instance['value'] = random.uniform(70, 100)
    output.write()
    sleep(20)