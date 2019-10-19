import re, services
import sims4.commands
import urllib
import urllib.request


@sims4.commands.Command('!vote', command_type=sims4.commands.CommandType.Live)
def startvote(_connection=None):
    try:
        output = sims4.commands.CheatOutput(_connection)
        tgt_client = services.client_manager().get(_connection)
        output("A vote on your household's Sims is starting! Type !endvote to close voting.")
        # with urllib.request.urlopen('http://www.python.org/') as f:
        #     output(f.read(300))
        all_sims = services.sim_info_manager().get_all()
        # output('{}'.format(repr(dir(all_sims))))
        # output('{}'.format(all_sims))
        household = tgt_client.household
        # output("HELLO")
        # response = urllib.request.urlopen('https://google.com/')
        # output("HI")
        # output(response.read())
        f = open("startvote.comfy", "w")
        f.write("1.0\n")
        for info in all_sims:
            if info.household is household:
                # output(repr(vars(info)))
                output(info.first_name)
                output(info.last_name)
                strGender = re.sub('^Gender\\.', '', str(info.gender), 1, re.IGNORECASE)
                output(strGender)
                careers = info.careers.values()
                output(str(len(careers)))
                crList = []
                for career in careers:
                    output(str(type(career).__name__))
                    crList.append( str(type(career).__name__) )
                f.write("{},{},{},{}\n".format(info.first_name, info.last_name, strGender, crList[ 0 ] if len(crList) > 0 else "" ))
        f.close()
    except Exception as e:
        output(str(e))

@sims4.commands.Command('!endvote', command_type=sims4.commands.CommandType.Live)
def endvote(_connection=None):
    try:
        output = sims4.commands.CheatOutput(_connection)
        tgt_client = services.client_manager().get(_connection)
        output("The votes are now closed and it's time to tally up the results. Type !results to see the poll results.")
        f = open("endvote.comfy", "w")
        f.write("1.0\n")
        f.close()
    except Exception as e:
        output(str(e))

@sims4.commands.Command('!results', command_type=sims4.commands.CommandType.Live)
def voteresults(_connection=None):
    try:
        output = sims4.commands.CheatOutput(_connection)
        tgt_client = services.client_manager().get(_connection)
        output("The votes are now closed and it's time to tally up the results. Type !results to see the poll results.")
        f = open("results.comfy", "r")
        contents = f.read()
        output(contents)
        f.close()
    except Exception as e:
        output(str(e))
